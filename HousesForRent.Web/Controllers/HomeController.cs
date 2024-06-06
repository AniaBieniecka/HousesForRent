using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Application.Services.Implementation;
using HousesForRent.Application.Services.Interface;
using HousesForRent.Domain.Entities;
using HousesForRent.Web.Models;
using HousesForRent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HousesForRent.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHouseService _houseService;
        private readonly IBookingService _bookingService;
        private readonly IAmenityService _amenityService;
        private readonly IHouseAmenityService _houseAmenityService;
        public HomeController(ILogger<HomeController> logger, IHouseService houseService, IBookingService bookingService, IAmenityService amenityService, IHouseAmenityService houseAmenityService)
        {
            _logger = logger;
            _houseService = houseService;
            _bookingService = bookingService;
            _amenityService = amenityService;
            _houseAmenityService = houseAmenityService;
        }

        public IActionResult Index()
        {
            var homeVM = new HomeVM()
            {
                NightsQty = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
                HouseVMList = new List<HouseVM>()

            };
            var HouseList = _houseService.GetAllHouses();

            foreach (var house in HouseList)
            {

                HouseVM vm = new()
                {
                    House = house,
                    AmenityList = _amenityService.GetAllAmenities().ToList(),
                    HouseAmenitiesIdList = _houseAmenityService.GetAllHouseAmenities(house.Id).Select(u => u.AmenityId).ToList()
                };

                homeVM.HouseVMList.Add(vm);
            }

            return View(homeVM);
        }
        [HttpPost]
        public IActionResult ShowHousesByDate(HomeVM homeVM)
        {
            var houseList = _houseService.GetAllHouses();

            foreach (var house in houseList)
            {

                HouseVM vm = new()
                {
                    House = house,
                    AmenityList = _amenityService.GetAllAmenities().ToList(),
                    HouseAmenitiesIdList = _houseAmenityService.GetAllHouseAmenities(house.Id).Select(u => u.AmenityId).ToList()
                };

                homeVM.HouseVMList.ToList().Add(vm);
            }


            var bookings = _bookingService.GetAllBookings("","Pending,Approved,CheckedIn").ToList();
                
            foreach (var houseVM in homeVM.HouseVMList)
            {
                houseVM.House.IsBooked = SD.isHouseBooked(houseVM.House.Id, homeVM.CheckInDate, homeVM.NightsQty, bookings);
            }

            return PartialView("_HouseGrid", homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
