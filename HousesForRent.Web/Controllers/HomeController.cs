using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
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
        private readonly IUnitOfWork _uniUnitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _uniUnitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var homeVM = new HomeVM()
            {
                HouseList = _uniUnitOfWork.House.GetAllHouses(),
                NightsQty = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),

            };

            return View(homeVM);
        }
        [HttpPost]
        public IActionResult ShowHousesByDate(HomeVM homeVM)
        {
            homeVM.HouseList = _uniUnitOfWork.House.GetAllHouses();
            var bookings = _uniUnitOfWork.Booking.GetAll(u => u.Status ==SD.StatusPending || 
            u.Status== SD.StatusApproved || u.Status == SD.StatusCheckedIn).ToList();

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
