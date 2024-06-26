﻿using Azure.Identity;
using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Application.Services.Implementation;
using HousesForRent.Application.Services.Interface;
using HousesForRent.Domain.Entities;
using HousesForRent.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HousesForRent.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBookingService _bookingService;
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, IBookingService bookingService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _bookingService = bookingService;
        }
        public IActionResult Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            LoginVM loginVM = new()
            {
                RedirectUrl = returnUrl
            };

            return View(loginVM);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(loginVM.Email);
                    if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(loginVM.RedirectUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return LocalRedirect(loginVM.RedirectUrl);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            return View(loginVM);
        }
        public IActionResult Register(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();
            }

            RegisterVM registerVM = new()
            {
                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }),
                RedirectUrl = returnUrl
            };

            return View(registerVM);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    Name = registerVM.Name,
                    Email = registerVM.Email,
                    PhoneNumber = registerVM.PhoneNumber,
                    NormalizedEmail = registerVM.Email.ToUpper(),
                    EmailConfirmed = true,
                    UserName = registerVM.Email,
                    CreatedAt = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, registerVM.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(registerVM.Role))
                    {
                        await _userManager.AddToRoleAsync(user, registerVM.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (string.IsNullOrEmpty(registerVM.RedirectUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return LocalRedirect(registerVM.RedirectUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            registerVM.RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            });

            return View(registerVM);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = SD.Role_Admin)]

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<UserVM>();
            var bookings = _bookingService.GetAllBookings().ToList();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserVM
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    Roles = userRoles,
                    BookingsQty = bookings.FindAll(u=>u.UserId ==  user.Id).Count(),
                });

            }
            return View(userList.ToList());
        }

        public IActionResult CustomerBookings(string id)
        {
            var bookingList = _bookingService.GetAllBookings(id).ToList();
            return View(bookingList);
        }

        public IActionResult ManageAccount()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ManageAccount(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var userFromDB = await _userManager.FindByIdAsync(user.Id);
                userFromDB.PhoneNumber = user.PhoneNumber;
                userFromDB.Name = user.Name;
                await _userManager.UpdateAsync(userFromDB);
                TempData["success"] = "The account was updated successfully";
                return RedirectToAction("Index", "Home");
            }

            else
                TempData["error"] = "The account wasn't updated successfully";

            return View(user);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
