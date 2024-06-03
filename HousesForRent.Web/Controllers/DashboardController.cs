using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Services.Implementation;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Application.Services.Interface;
using HousesForRent.Domain.Entities;
using HousesForRent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.AspNetCore.Identity;
using static HousesForRent.Web.ViewModels.DashboardVM;

namespace HousesForRent.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IBookingService _bookingService;
        public DashboardController(IDashboardService dashboardService, IBookingService bookingService)
        {
            _dashboardService = dashboardService;
            _bookingService = bookingService;
        }
        public IActionResult Index()
        {

            DashboardVM vm = new ()
            {
                lastBookings = _bookingService.GetAllBookings().OrderByDescending(u => u.BookingDate).Take(3).Select(u => new BookingDto
                {
                    bookingDate = DateOnly.FromDateTime(u.BookingDate),
                   houseName= u.House.Name,
                   cost = u.Cost
                }).ToList(),
            };

            return View(vm);
        }

        public async Task<IActionResult> GetTotalBookingChartData()
        {
            return Json(await _dashboardService.GetTotalBookingChartData());
        }

        public async Task<IActionResult> GetTotalUserChartData()
        {
            return Json(await _dashboardService.GetTotalUserChartData());
        }

        public async Task<IActionResult> GetTotalIncomeChartData()
        {
            return Json(await _dashboardService.GetTotalIncomeChartData());
        }

        public async Task<IActionResult> GetCustomerBookingPieChartData()
        {
            return Json(await _dashboardService.GetCustomerBookingPieChartData());
        }

        public async Task<IActionResult> GetCustomerAndBookingLineChartData()
        {
            return Json(await _dashboardService.GetCustomerAndBookingLineChartData());
        }
    }
}
