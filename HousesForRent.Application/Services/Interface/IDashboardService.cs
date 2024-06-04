using HousesForRent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Services.Interface
{
    public interface IDashboardService
    {

        Task<RadialBarChartDTO> GetTotalBookingChartData();
        Task<RadialBarChartDTO> GetTotalUserChartData();
        Task<RadialBarChartDTO> GetTotalIncomeChartData();
        Task<PieChartDTO> GetCustomerBookingPieChartData();
        Task<LineChartDTO> GetCustomerAndBookingLineChartData();
        Task<BarChartDTO> GetIncomeAndBookingBarChartData();

    }
}
