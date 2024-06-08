using HousesForRent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Services.Interface
{
    public interface IBookingService
    {
        IEnumerable<Booking> GetAllBookings(string? userId="", string? statusFilterList = "");
        Booking GetBooking(int id);
        void CreateBooking(Booking booking);
        bool IsHouseBooked(int houseId, int nightsQty, DateOnly checkInDate);
        void UpdateStatus(int bookingId, string bookingStatus);
        void UpdateStripePaymentID(int bookingId, string sesstionId, string paymentIntentId);

    }
}
