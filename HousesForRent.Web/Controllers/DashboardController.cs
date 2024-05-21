using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Services.Implementation;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Application.Services.Interface;
using HousesForRent.Domain.Entities;
using HousesForRent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HousesForRent.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public IActionResult Index()
        {
            return View();
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
