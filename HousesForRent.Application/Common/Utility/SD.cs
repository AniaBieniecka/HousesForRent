using HousesForRent.Domain.Entities;
using HousesForRent.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Common.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusCheckedIn = "CheckedIn";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static bool isHouseBooked(int houseId, DateOnly wantedCheckInDate, int nightsQty, List<Booking> bookings)
        {
            DateOnly wantedCheckOutDate = wantedCheckInDate.AddDays(nightsQty);
            var bookingsForSelectedHouse = bookings.Where(u=>u.HouseId== houseId).ToList();

            foreach (var booking in bookingsForSelectedHouse)
            {
                // if wanted checkindate is between booked dates
                if (wantedCheckInDate >= booking.CheckInDate && wantedCheckInDate < booking.CheckOutDate)
                {
                    return true; 
                }
                // if wanted checkoutdate is between booked dates
                if (wantedCheckOutDate > booking.CheckInDate && wantedCheckOutDate <= booking.CheckOutDate)
                {
                    return true; 
                }
                // if wanted booking period inlcudes booking
                if (wantedCheckInDate <= booking.CheckInDate && wantedCheckOutDate >= booking.CheckOutDate)
                {
                    return true; 
                }
            }
            return false;

        }

        public static RadialBarChartDTO GetRadialChartViewModel(int totalCount, double countByCurrentMonth, double countByPreviousMonth)
        {
            RadialBarChartDTO RadialBarChartDTO = new();

            int increaseDecreaseRatio = 100;

            if (countByPreviousMonth != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((countByCurrentMonth - countByPreviousMonth) / countByPreviousMonth * 100);
            }

            RadialBarChartDTO.TotalCount = totalCount;
            RadialBarChartDTO.CountCurrentMonth = Convert.ToInt32(countByCurrentMonth);
            RadialBarChartDTO.MonthlyChange = Convert.ToInt32(countByCurrentMonth - countByPreviousMonth);
            RadialBarChartDTO.Series = new int[] { increaseDecreaseRatio };

            return RadialBarChartDTO;
        }

    }
}
