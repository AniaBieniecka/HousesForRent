using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HousesForRent.Web.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public AmenityController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviroment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult Index()
        {
            var amenities = _unitOfWork.Amenity.GetAll().ToList();
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

                _unitOfWork.Amenity.Add(amenity);
                _unitOfWork.Amenity.Save();
                TempData["success"] = "The amenity was created successfully";

                return RedirectToAction("Index");
            }
            else return View(amenity);
        }

        public IActionResult Update(int id)
        {
            var amenity = _unitOfWork.Amenity.Get(u => u.Id == id);
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
                _unitOfWork.Amenity.Update(amenity);
                _unitOfWork.Amenity.Save();
                TempData["success"] = "The amenity was updated successfully";
                return RedirectToAction("Index");

            }
            else
                TempData["error"] = "The house wasn't updated successfully";

            return View(amenity);
        }

        public IActionResult Delete(int id)
        {
            var amenity = _unitOfWork.Amenity.Get(u => u.Id == id);
            if (amenity is not null)
            {
                return View(amenity);
            }
            else return NotFound();
        }
        [HttpPost]
        public IActionResult Delete(Amenity obj)
        {
            Amenity? amenity = _unitOfWork.Amenity.Get(u => u.Id == obj.Id);

            if (amenity is not null)
            {
                _unitOfWork.Amenity.Remove(amenity);
                _unitOfWork.Amenity.Save();
                TempData["success"] = "The amenity was deleted successfully";
                return RedirectToAction("Index");
            }
            else
                TempData["error"] = "The amenity wasn't deleted successfully";
            return View();
        }
    }
}
