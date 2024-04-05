using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HousesForRent.Web.Controllers
{
    public class HouseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HouseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            if(house.Price < house.DiscountPrice)
            {
                ModelState.AddModelError("DiscountPrice", "The discount price cannot be higer than standard price");
            }
            if(ModelState.IsValid)
            {
            _unitOfWork.House.Add(house);
            _unitOfWork.House.Save();
            return RedirectToAction("Index");
            }
            else return View(house);
        }
    }
}
