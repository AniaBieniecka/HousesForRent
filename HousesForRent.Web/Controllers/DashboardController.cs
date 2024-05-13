using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Domain.Entities;
using HousesForRent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HousesForRent.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending || u.Status == SD.StatusCancelled);

            double countByCurrentMonth = totalBookings.Count(u => u.BookingDate >= currentMonthStartDate &&
            u.BookingDate <= DateTime.Now);

            double countByPreviousMonth = totalBookings.Count(u => u.BookingDate >= previousMonthStartDate &&
            u.BookingDate <= currentMonthStartDate);

            var radialBarChartVM = GetRadialChartViewModel(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth);

            return Json(radialBarChartVM);
        }

        public async Task<IActionResult> GetTotalUserChartData()
        {
            var totalUsers = _unitOfWork.ApplicationUser.GetAll();

            double countByCurrentMonth = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate &&
            u.CreatedAt <= DateTime.Now);

            double countByPreviousMonth = totalUsers.Count(u => u.CreatedAt >= previousMonthStartDate &&
            u.CreatedAt <= currentMonthStartDate);

            var radialBarChartVM = GetRadialChartViewModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth);

            return Json(radialBarChartVM);
        }

        public async Task<IActionResult> GetTotalIncomeChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending || u.Status == SD.StatusCancelled);

            double sumByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate &&
            u.BookingDate <= DateTime.Now).Sum(u => u.Cost);

            double sumByPreviousMonth = totalBookings.Where(u => u.BookingDate >= previousMonthStartDate &&
            u.BookingDate <= currentMonthStartDate).Sum(u => u.Cost);

            var radialBarChartVM = GetRadialChartViewModel(Convert.ToInt32(totalBookings.Sum(u => u.Cost)), sumByCurrentMonth, sumByPreviousMonth);

            return Json(radialBarChartVM);
        }

        public async Task<IActionResult> GetCustomerBookingPieChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) &&
            (u.Status != SD.StatusPending || u.Status == SD.StatusCancelled));

            var customerWithOneBookingCount = totalBookings.GroupBy(u => u.UserId).Where(x => x.Count() == 1).Count();
            var customerWithMoreBookingsCount = totalBookings.Count() - customerWithOneBookingCount;

            PieChartVM pieChartVM = new()
            {
                Series = new int[] { customerWithOneBookingCount, customerWithMoreBookingsCount },
                Labels = new string[] { "New customers", "Returning customers" }
            };

            return Json(pieChartVM);
        }

        public async Task<IActionResult> GetCustomerAndBookingLineChartData()
        {

            var bookings = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) &&
            (u.Status != SD.StatusPending || u.Status == SD.StatusCancelled))
                .GroupBy(u => u.BookingDate.Date)
                .Select(x => new
                {
                    DateTime = x.Key,
                    BookingCount = x.Count(),
                });

            var users = _unitOfWork.ApplicationUser.GetAll(u => u.CreatedAt >= DateTime.Now.AddDays(-30))
            .GroupBy(u => u.CreatedAt.Date)
            .Select(x => new
            {
                DateTime = x.Key,
                UserCount = x.Count(),
            });

            var leftJoin = bookings.GroupJoin(users, booking => booking.DateTime, user => user.DateTime,
                (booking, user) => new
                {
                    booking.DateTime,
                    booking.BookingCount,
                    UserCount = user.Select(x => x.UserCount).FirstOrDefault()
                });

            var rightJoin = users.GroupJoin(bookings, user => user.DateTime, booking => booking.DateTime,
                (user, booking) => new
                {
                    user.DateTime,
                    BookingCount = bookings.Select(x => x.BookingCount).FirstOrDefault(),
                    user.UserCount

                });

            var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();

            var newBookingData = mergedData.Select(x => x.BookingCount).ToArray();
            var newUserData = mergedData.Select(x=>x.UserCount).ToArray();
            var categories = mergedData.Select(x => x.DateTime.ToString("MM/dd/yyyy")).ToArray();

            List<ChartData> chartDataList = new()
            {
                new ChartData
                {
                    Name = "New bookings",
                    Data = newBookingData
                },
                new ChartData
                {
                    Name = "New customers",
                    Data = newUserData
                }
            };

            LineChartVM lineChartVM = new()
            {
                Categories = categories,
                Series = chartDataList
            };

            return Json(lineChartVM);
        }
        private static RadialBarChartVM GetRadialChartViewModel(int totalCount, double countByCurrentMonth, double countByPreviousMonth)
        {
            RadialBarChartVM radialBarChartVM = new();

            int increaseDecreaseRatio = 100;

            if (countByPreviousMonth != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((countByCurrentMonth - countByPreviousMonth) / countByPreviousMonth * 100);
            }

            radialBarChartVM.TotalCount = totalCount;
            radialBarChartVM.CountCurrentMonth = Convert.ToInt32(countByCurrentMonth);
            radialBarChartVM.MonthlyChange = Convert.ToInt32(countByCurrentMonth - countByPreviousMonth);
            radialBarChartVM.Series = new int[] { increaseDecreaseRatio };

            return radialBarChartVM;
        }
    }
}
