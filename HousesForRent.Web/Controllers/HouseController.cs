using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HousesForRent.Web.Controllers
{
    public class HouseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public HouseController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviroment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult Index()
        {
            var houses = _unitOfWork.House.GetAll().ToList();
            return View(houses);
        }


        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(House house)
        {
            if (house.Price < house.DiscountPrice)
            {
                ModelState.AddModelError("DiscountPrice", "The discount price cannot be higer than standard price");
            }
            if (ModelState.IsValid)
            {
                if(house.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(house.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnviroment.WebRootPath, @"images\House");

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                        house.Image.CopyTo(fileStream);

                    house.ImageUrl = @"\images\House\" + fileName;
                }
                _unitOfWork.House.Add(house);
                _unitOfWork.House.Save();
                TempData["success"] = "The house was created successfully";

                return RedirectToAction("Index");
            }
            else return View(house);
        }

        public IActionResult Update(int id)
        {
            var house = _unitOfWork.House.Get(u => u.Id == id);
            if (house is not null)
            {
                return View(house);
            }
            else return NotFound();
        }
        [HttpPost]
        public IActionResult Update(House house)
        {
            if (house.Price < house.DiscountPrice)
            {
                ModelState.AddModelError("DiscountPrice", "The discount price cannot be higer than standard price");
            }
            if (ModelState.IsValid)
            {
                if (house.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(house.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnviroment.WebRootPath, @"images\House");

                    if (!string.IsNullOrEmpty(house.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnviroment.WebRootPath, house.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                        house.Image.CopyTo(fileStream);

                    house.ImageUrl = @"\images\House\" + fileName;
                }

                _unitOfWork.House.Update(house);
                _unitOfWork.House.Save();
                TempData["success"] = "The house was updated successfully";
                return RedirectToAction("Index");

            }
            else
                TempData["error"] = "The house wasn't deleted successfully";

            return View(house);
        }

        public IActionResult Delete(int id)
        {
            var house = _unitOfWork.House.Get(u => u.Id == id);
            if (house is not null)
            {
                return View(house);
            }
            else return NotFound();
        }
        [HttpPost]
        public IActionResult Delete(House obj)
        {
            House? house = _unitOfWork.House.Get(u => u.Id == obj.Id);

            if (house is not null)
            {
                if (!string.IsNullOrEmpty(house.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnviroment.WebRootPath, house.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.House.Remove(house);
                _unitOfWork.House.Save();
                TempData["success"] = "The house was deleted successfully";
                return RedirectToAction("Index");
            }
            else
                TempData["error"] = "The house wasn't deleted successfully";
            return View();
        }
    }
}
