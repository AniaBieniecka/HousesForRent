using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Application.Services.Interface;
using HousesForRent.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<LineChartDTO> GetCustomerAndBookingLineChartData()
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
            var newUserData = mergedData.Select(x => x.UserCount).ToArray();
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

            LineChartDTO lineChartDTO = new()
            {
                Categories = categories,
                Series = chartDataList
            };

            return lineChartDTO;
        }

        public async Task<PieChartDTO> GetCustomerBookingPieChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) &&
            (u.Status != SD.StatusPending || u.Status == SD.StatusCancelled));

            var customerWithOneBookingCount = totalBookings.GroupBy(u => u.UserId).Where(x => x.Count() == 1).Count();
            var customerWithMoreBookingsCount = totalBookings.Count() - customerWithOneBookingCount;

            PieChartDTO PieChartDTO = new()
            {
                Series = new int[] { customerWithOneBookingCount, customerWithMoreBookingsCount },
                Labels = new string[] { "New customers", "Returning customers" }
            };

            return PieChartDTO;
        }

        public async Task<RadialBarChartDTO> GetTotalBookingChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending || u.Status == SD.StatusCancelled);

            double countByCurrentMonth = totalBookings.Count(u => u.BookingDate >= currentMonthStartDate &&
            u.BookingDate <= DateTime.Now);

            double countByPreviousMonth = totalBookings.Count(u => u.BookingDate >= previousMonthStartDate &&
            u.BookingDate < currentMonthStartDate);

            var RadialBarChartDTO = SD.GetRadialChartViewModel(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth);

            return RadialBarChartDTO;
        }

        public async Task<RadialBarChartDTO> GetTotalIncomeChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending || u.Status == SD.StatusCancelled);

            double sumByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate &&
            u.BookingDate <= DateTime.Now).Sum(u => u.Cost);

            double sumByPreviousMonth = totalBookings.Where(u => u.BookingDate >= previousMonthStartDate &&
            u.BookingDate < currentMonthStartDate).Sum(u => u.Cost);

            var RadialBarChartDTO = SD.GetRadialChartViewModel(Convert.ToInt32(totalBookings.Sum(u => u.Cost)), sumByCurrentMonth, sumByPreviousMonth);

            return RadialBarChartDTO;
        }

        public async Task<RadialBarChartDTO> GetTotalUserChartData()
        {
            var totalUsers = _unitOfWork.ApplicationUser.GetAll();

            double countByCurrentMonth = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate &&
            u.CreatedAt <= DateTime.Now);

            double countByPreviousMonth = totalUsers.Count(u => u.CreatedAt >= previousMonthStartDate &&
            u.CreatedAt < currentMonthStartDate);

            var RadialBarChartDTO = SD.GetRadialChartViewModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth);

            return RadialBarChartDTO;
        }

        public async Task<BarChartDTO> GetIncomeAndBookingBarChartData()
        {
            var bookings = _unitOfWork.Booking.GetAll(u=>(u.Status != SD.StatusPending || u.Status != SD.StatusCancelled)&&(u.BookingDate.Year==DateTime.Now.Year))
                .GroupBy(u => u.BookingDate.ToString("MMMM"))
                .Select(x => new
                {
                    Month = x.Key,
                    BookingsCount = (double)x.Count(),
                    BookingsCost = x.Sum(i=>i.Cost)
                });

            var newBookingData = bookings.Select(x => x.BookingsCount).ToArray();
            var incomeData = bookings.Select(x => x.BookingsCost).ToArray();
            var categories = bookings.Select(x => x.Month).ToArray();

            List<BarChartData> chartDataList = new()
            {
                new BarChartData
                {
                    Name = "New bookings",
                    Type = "column",
                    Data = newBookingData,
                    Unit = ""

                },
                new BarChartData
                {
                    Name = "Income",
                    Type = "column",
                    Data = incomeData,
                    Unit = "PLN"
                }
            };

            BarChartDTO barchartDTO = new()
            {
                Categories = categories,
                Series = chartDataList
            };

            return barchartDTO;
        }

    }
}
