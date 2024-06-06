using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Application.Services.Interface;
using HousesForRent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Services.Implementation
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void CreateBooking(Booking booking)
        {
            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Booking.Save();
        }

        public IEnumerable<Booking> GetAllBookings(string? userId = "", string? statusFilterList = "")
        {
            IEnumerable<string> statusList = statusFilterList.ToLower().Split(",");
            if(!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(statusFilterList))
            {
                return _unitOfWork.Booking.GetAll(u=>statusList.Contains(u.Status.ToLower()) && u.UserId == userId, includeProperties: "User,House");
            }
            else
            {
                if (!string.IsNullOrEmpty(statusFilterList))
                {
                    return _unitOfWork.Booking.GetAll(u => statusList.Contains(u.Status.ToLower()), includeProperties: "User,House");
                }
                if(!string.IsNullOrEmpty(userId))
                {
                    return _unitOfWork.Booking.GetAll(u => u.UserId == userId, includeProperties: "User,House");
                }
            }
            return _unitOfWork.Booking.GetAll(includeProperties: "User,House");
        }

        public Booking GetBooking(int id)
        {
            return _unitOfWork.Booking.Get(u=>u.Id==id, includeProperties: "User,House");
        }

        public bool IsHouseBooked(int houseId, int nightsQty, DateOnly checkInDate)
        {
            var bookings = _unitOfWork.Booking.GetAll(u => (u.Status == SD.StatusPending ||
            u.Status == SD.StatusApproved || u.Status == SD.StatusCheckedIn) && u.HouseId == houseId).ToList();
            return  SD.isHouseBooked(houseId, checkInDate, nightsQty, bookings);
        }

        public void UpdateStatus(int bookingId, string bookingStatus)
        {
            var bookingFromDB = _unitOfWork.Booking.Get(u => u.Id == bookingId);
            if (bookingFromDB != null)
            {
                bookingFromDB.Status = bookingStatus;

                if (bookingStatus == SD.StatusCheckedIn)
                {
                    bookingFromDB.ActualCheckInDate = DateTime.Now;
                }
                if (bookingStatus == SD.StatusCompleted)
                {
                    bookingFromDB.ActualCheckOutDate = DateTime.Now;
                }
            }
            _unitOfWork.Booking.Update(bookingFromDB);
            _unitOfWork.Booking.Save();
        }

        public void UpdateStripePaymentID(int bookingId, string sesstionId, string paymentIntentId)
        {
            var bookingFromDB = _unitOfWork.Booking.Get(u => u.Id == bookingId);
            if (bookingFromDB != null)
            {
                if (!string.IsNullOrEmpty(sesstionId))
                {
                    bookingFromDB.StripeSessionId = sesstionId;
                }
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    bookingFromDB.StripePaymentIntentId = paymentIntentId;
                    bookingFromDB.PaymentDate = DateTime.Now;
                    bookingFromDB.IsPaymentSuccessful = true;
                }
            }

            _unitOfWork.Booking.Update(bookingFromDB);
            _unitOfWork.Booking.Save();
        }
    }
}
