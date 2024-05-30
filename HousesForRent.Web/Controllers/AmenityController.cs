using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Application.Services.Interface;
using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HousesForRent.Web.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IAmenityService _amenityService;
        public AmenityController(IAmenityService amenityService)
        {
            _amenityService = amenityService;
        }
        public IActionResult Index()
        {
            var amenities = _amenityService.GetAllAmenities().ToList();
            return View(amenities);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Amenity amenity)
        {
            if (ModelState.IsValid)
            {
                _amenityService.CreateAmenity(amenity);
                TempData["success"] = "The amenity was created successfully";

                return RedirectToAction("Index");
            }
            else return View(amenity);
        }

        public IActionResult Update(int id)
        {
            var amenity = _amenityService.GetAmenity(id);
            if (amenity is not null)
            {
                return View(amenity);
            }
            else return NotFound();
        }
        [HttpPost]
        public IActionResult Update(Amenity amenity)
        {

            if (ModelState.IsValid)
            {
                _amenityService.UpdateAmenity(amenity);
                TempData["success"] = "The amenity was updated successfully";
                return RedirectToAction("Index");

            }
            else
                TempData["error"] = "The house wasn't updated successfully";

            return View(amenity);
        }

        public IActionResult Delete(int id)
        {
            var amenity = _amenityService.GetAmenity(id);
            if (amenity is not null)
            {
                return View(amenity);
            }
            else return NotFound();
        }
        [HttpPost]
        public IActionResult Delete(Amenity obj)
        {
            Amenity? amenity = _amenityService.GetAmenity(obj.Id);

            if (amenity is not null)
            {
                _amenityService.DeleteAmenity(obj.Id);
                TempData["success"] = "The amenity was deleted successfully";
                return RedirectToAction("Index");
            }
            else
                TempData["error"] = "The amenity wasn't deleted successfully";
            return View();
        }
    }
}
