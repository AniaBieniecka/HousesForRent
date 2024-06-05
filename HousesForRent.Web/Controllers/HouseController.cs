using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Application.Services.Interface;
using HousesForRent.Domain.Entities;
using HousesForRent.Infrastructure.Repository;
using HousesForRent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;

namespace HousesForRent.Web.Controllers
{
    public class HouseController : Controller
    {
        private readonly IHouseService _houseService;
        private readonly IAmenityService _amenityService;
        private readonly IHouseAmenityService _houseAmenityService;
        private readonly IBookingService _bookingService;
        public HouseController(IHouseService houseService, IAmenityService amenityService, IHouseAmenityService houseAmenityService, IBookingService bookingService)
        {
            _houseService = houseService;
            _amenityService = amenityService;
            _houseAmenityService = houseAmenityService;
            _bookingService = bookingService;
        }
        public IActionResult Index()
        {
            var houses = _houseService.GetAllHouses();
            return View(houses);
        }


        public IActionResult Create()
        {
            HouseVM houseVM = new()
            {
                AmenityList = _amenityService.GetAllAmenities().ToList(),
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
                house.Occupancy = house.SingleBedQuantity + house.DoubleBedQuantity * 2;
                _houseService.CreateHouse(house);

                if (amenityId is not null)
                {
                    foreach (var id in amenityId)
                    {
                        HouseAmenity houseAmenity = new HouseAmenity();
                        houseAmenity.HouseId = house.Id;
                        houseAmenity.AmenityId = id;

                        _houseAmenityService.CreateHouseAmenity(houseAmenity);
                    }
                }
                TempData["success"] = "The house was created successfully";

                return RedirectToAction("Index");
            }
            else return View(house);
        }

        public IActionResult Update(int id)
        {
            var house = _houseService.GetHouse(id);

            HouseVM vm = new()
            {
                House = house,
                AmenityList = _amenityService.GetAllAmenities().ToList(),
                HouseAmenitiesIdList = _houseAmenityService.GetAllHouseAmenities(id).Select(u => u.AmenityId).ToList()
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
                houseVM.House.Occupancy = houseVM.House.SingleBedQuantity + houseVM.House.DoubleBedQuantity * 2;

                _houseService.UpdateHouse(houseVM.House);

                // updating HouseAmenity
                var existingAmenityIdList = _houseAmenityService.GetAllHouseAmenities(houseVM.House.Id).Select(u => u.AmenityId).ToList();
                var selectedAmenityIdList = amenityId;

                var amenitiesToRemove = existingAmenityIdList.Except(selectedAmenityIdList);

                foreach (var amenityToRemoveId in amenitiesToRemove)
                {
                    var houseAmenityToRemove = _houseAmenityService.GetHouseAmenity(houseVM.House.Id, amenityToRemoveId);
                    _houseAmenityService.DeleteHouseAmenity(houseAmenityToRemove.Id);
                }

                var amenitiesToAdd = selectedAmenityIdList.Except(existingAmenityIdList);

                foreach (var amenityToAddId in amenitiesToAdd)
                {
                    HouseAmenity houseAmenityToAdd = new()
                    {
                        AmenityId = amenityToAddId,
                        HouseId = houseVM.House.Id
                    };

                    _houseAmenityService.CreateHouseAmenity(houseAmenityToAdd);
                }
                TempData["success"] = "The house was updated successfully";
                return RedirectToAction("Index");
            }

            else
                TempData["error"] = "The house wasn't updated successfully";

            return View(houseVM);

        }

        public IActionResult Delete(int id)
        {
            var house = _houseService.GetHouse(id);

            HouseVM vm = new()
            {
                House = house,
                AmenityList = _amenityService.GetAllAmenities().ToList(),
                HouseAmenitiesIdList = _houseAmenityService.GetAllHouseAmenities(id).Select(u => u.AmenityId).ToList()

            };

            if (house is not null)
            {
                return View(vm);
            }
            else return NotFound();
        }
        [HttpPost]
        public IActionResult Delete(HouseVM vm)
        {;
            House? house = _houseService.GetHouse(vm.House.Id);

            if (house is not null)
            {
                _houseService.DeleteHouse(house.Id);

                TempData["success"] = "The house was deleted successfully";
                return RedirectToAction("Index");
            }
            else
                TempData["error"] = "The house wasn't deleted successfully";
            return View();
        }

        [HttpGet]
        public IActionResult CheckAvailability()
        {
            var houseList = _houseService.GetAllHouses();
            return View(houseList);
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarBookingData(string? selectedHouses)
        {
            List<int> houseIdList = new List<int>();

            if (selectedHouses is not null)
            {
                var selectedResourceArray = JsonConvert.DeserializeObject<string[]>(selectedHouses);
                houseIdList = selectedResourceArray.Select(s => Convert.ToInt32(s)).ToList();

            }
            else houseIdList = _houseService.GetAllHouses().Select(u=>u.Id).Take(6).ToList();                   

            IList<Booking> bookingList = new List<Booking>();

            foreach (var id in houseIdList)
            {
                var booking = _bookingService.GetAllBookings("", "Pending,Approved,CheckedIn,Completed,Refunded").Where(a=>a.HouseId== id).ToList();        
                bookingList = bookingList.Concat(booking).ToList();
            }

            var eventList = bookingList
            .Select(x => new { id = x.Id, resourceId = x.HouseId, title = x.House.Name + ", booking Id: " + x.Id, start = x.CheckInDate, end = x.CheckOutDate })
            .ToList();

            return Json(eventList);
        }


    }
}