using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Application.Services.Interface;
using HousesForRent.Domain.Entities;
using HousesForRent.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;
using System.Security.Claims;

namespace HousesForRent.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IHouseService _houseService;
        private readonly UserManager<ApplicationUser> _userManager;
        public BookingController(IBookingService bookingService, IHouseService houseService, UserManager<ApplicationUser> userManager)
        {
            _bookingService = bookingService;
            _houseService = houseService;
            _userManager = userManager;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult ProcessBooking(int houseId, int nightsQty, string checkInDate)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();

            DateOnly.TryParseExact(checkInDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateOnly checkInDateParsed);

            Booking booking = new()
            {
                HouseId = houseId,
                House = _houseService.GetHouse(houseId),
                CheckInDate = checkInDateParsed,
                CheckOutDate = checkInDateParsed.AddDays(nightsQty),
                NightsQty = nightsQty,
                UserId = userId,
                UserName = user.Name,
                UserEmail = user.Email,
                Phone = user.PhoneNumber
            };
            booking.Cost = booking.House.Price * nightsQty;

            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProcessBooking(Booking booking)
        {
            var house = _houseService.GetHouse(booking.HouseId);

            booking.Cost = house.Price * booking.NightsQty;
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;

            house.IsBooked = _bookingService.IsHouseBooked(house.Id,booking.NightsQty, booking.CheckInDate);

            if(house.IsBooked)
            {
                TempData["error"] = "House has just been sold out";
                return RedirectToAction(nameof(ProcessBooking), new
                {
                    houseId=booking.HouseId,
                    checkInDate = booking.CheckInDate,
                    nightsQty = booking.NightsQty
                });
            }

            _bookingService.CreateBooking(booking);

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                Mode = "payment",
                SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"booking/ProcessBooking?houseId={booking.HouseId}&checkInDate={booking.CheckInDate}&nights={booking.NightsQty}",
            };

            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.Cost * 100),
                    Currency = "pln",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = house.Name
                    },
                },
                Quantity = 1,
            });

            var service = new SessionService();
            Session session = service.Create(options);

            _bookingService.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            Booking bookingFromDB = _bookingService.GetBooking(bookingId);
            if (bookingFromDB.Status == SD.StatusPending)
            {
                //confirming if payment was successful
                var service = new SessionService();
                Session session = service.Get(bookingFromDB.StripeSessionId);

                if (session.PaymentStatus == "paid")
                {
                    _bookingService.UpdateStatus(bookingFromDB.Id, SD.StatusApproved);
                    _bookingService.UpdateStripePaymentID(bookingFromDB.Id, session.Id, session.PaymentIntentId);
                }
            }
            return View(bookingId);
        }

        [Authorize]
        public  IActionResult BookingDetails (int bookingId)
        {
            Booking bookingFromDB = _bookingService.GetBooking(bookingId);
            return View(bookingFromDB);
        }

        [HttpPost]
        [Authorize(Roles =SD.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _bookingService.UpdateStatus(booking.Id, SD.StatusCheckedIn);
            TempData["Success"] = "Booking updated successfully";
            return RedirectToAction(nameof(BookingDetails), new {bookingId = booking.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            _bookingService.UpdateStatus(booking.Id, SD.StatusCompleted);
            TempData["Success"] = "Booking completed successfully";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Cancel(Booking booking)
        {
            _bookingService.UpdateStatus(booking.Id, SD.StatusCancelled);
            TempData["Success"] = "Booking cancelled successfully";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });

        }
        #region API CALLS
        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> objBookings;

            if (User.IsInRole(SD.Role_Admin)){
                objBookings = _bookingService.GetAllBookings();
            } else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objBookings = _bookingService.GetAllBookings(userId);
            }
            if (!string.IsNullOrEmpty(status))
            {
                objBookings = objBookings.Where(u=>u.Status == status);
            }
            return Json(new { data=objBookings });
        }

        #endregion
    }
}
