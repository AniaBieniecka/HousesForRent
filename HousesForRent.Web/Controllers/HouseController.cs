using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Domain.Entities;
using HousesForRent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using System.Linq;
using System.Linq.Expressions;

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
            HouseVM houseVM = new()
            {
                AmenityList = _unitOfWork.Amenity.GetAll().ToList()
            };

            return View(houseVM);
        }
        [HttpPost]
        public IActionResult Create(House house, int[] amenityId)
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

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                        house.Image.CopyTo(fileStream);

                    house.ImageUrl = @"\images\House\" + fileName;
                }

                house.Occupancy = house.SingleBedQuantity + house.DoubleBedQuantity * 2;

                _unitOfWork.House.Add(house);
                _unitOfWork.House.Save();

                if (amenityId is not null)
                {
                    foreach (var id in amenityId)
                    {
                        HouseAmenity houseAmenity = new HouseAmenity();
                        houseAmenity.HouseId = house.Id;
                        houseAmenity.AmenityId = id;
                        _unitOfWork.HouseAmenity.Add(houseAmenity);
                        _unitOfWork.House.Save();
                    }
                }
                TempData["success"] = "The house was created successfully";

                return RedirectToAction("Index");
            }
            else return View(house);
        }

        public IActionResult Update(int id)
        {
            var house = _unitOfWork.House.Get(u => u.Id == id);

            HouseVM vm = new()
            {
                House = house,
                AmenityList = _unitOfWork.Amenity.GetAll().ToList(),
                HouseAmenitiesIdList = _unitOfWork.HouseAmenity.GetAll(u => u.HouseId == id).Select(u => u.AmenityId).ToList()

            };

            if (house is not null)
            {
                return View(vm);
            }
            else return NotFound();
        }
        [HttpPost]
        public IActionResult Update(HouseVM houseVM, int[] amenityId)
        {
            if (houseVM.House.Price < houseVM.House.DiscountPrice)
            {
                ModelState.AddModelError("DiscountPrice", "The discount price cannot be higer than standard price");
            }
            if (ModelState.IsValid)
            {
                if (houseVM.House.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(houseVM.House.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnviroment.WebRootPath, @"images\House");

                    if (!string.IsNullOrEmpty(houseVM.House.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnviroment.WebRootPath, houseVM.House.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                        houseVM.House.Image.CopyTo(fileStream);

                    houseVM.House.ImageUrl = @"\images\House\" + fileName;
                }

                _unitOfWork.House.Update(houseVM.House);

                // updating HouseAmenity
                var existingAmenityIdList = _unitOfWork.HouseAmenity.GetAll(u => u.HouseId == houseVM.House.Id).Select(u=>u.AmenityId).ToList();
                var selectedAmenityIdList = amenityId;

                var amenitiesToRemove = existingAmenityIdList.Except(selectedAmenityIdList);

                foreach (var item in amenitiesToRemove)
                {
                    var houseAmenityToRemove = _unitOfWork.HouseAmenity.Get(u => u.AmenityId == item && u.HouseId == houseVM.House.Id);
                    _unitOfWork.HouseAmenity.Remove(houseAmenityToRemove);
                }
                
                var amenitiesToAdd = selectedAmenityIdList.Except(existingAmenityIdList);

                foreach (var item in amenitiesToAdd)
                {
                    HouseAmenity houseAmenityToAdd = new()
                    {
                        AmenityId = item,
                        HouseId = houseVM.House.Id
                    };
                    _unitOfWork.HouseAmenity.Add(houseAmenityToAdd);
                }

                _unitOfWork.House.Save();
                TempData["success"] = "The house was updated successfully";
                return RedirectToAction("Index");

            }
            else
                TempData["error"] = "The house wasn't deleted successfully";

            return View(houseVM);
        }

        public IActionResult Delete(int id)
        {
            var house = _unitOfWork.House.Get(u => u.Id == id);

            HouseVM vm = new()
            {
                House = house,
                AmenityList = _unitOfWork.Amenity.GetAll().ToList(),
                HouseAmenitiesIdList = _unitOfWork.HouseAmenity.GetAll(u => u.HouseId == id).Select(u => u.AmenityId).ToList()

            };

            if (house is not null)
            {
                return View(vm);
            }
            else return NotFound();
        }
        [HttpPost]
        public IActionResult Delete(HouseVM vm)
        {
            House? house = _unitOfWork.House.Get(u => u.Id == vm.House.Id);

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
