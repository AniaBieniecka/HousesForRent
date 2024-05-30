using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
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
        public HomeController(ILogger<HomeController> logger, IHouseService houseService, IBookingService bookingService)
        {
            _logger = logger;
            _houseService = houseService;
            _bookingService = bookingService;
        }

        public IActionResult Index()
        {
            var homeVM = new HomeVM()
            {
                HouseList = _houseService.GetAllHouses(),
                NightsQty = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),

            };

            return View(homeVM);
        }
        [HttpPost]
        public IActionResult ShowHousesByDate(HomeVM homeVM)
        {
            homeVM.HouseList = _houseService.GetAllHouses();
            var bookings = _bookingService.GetAllBookings("","Pending,Approved,CheckedIn").ToList();
                
            foreach (var house in homeVM.HouseList)
            {
                house.IsBooked = SD.isHouseBooked(house.Id, homeVM.CheckInDate, homeVM.NightsQty, bookings);
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
