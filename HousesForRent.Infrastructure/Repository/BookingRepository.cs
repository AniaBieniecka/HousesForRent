using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Domain.Entities;
using HousesForRent.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _db;
        public BookingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Booking booking)
        {
            _db.Update(booking);
        }

        public void UpdateStatus(int bookingId, string bookingStatus)
        {
            var bookingFromDB = _db.Bookings.FirstOrDefault(u => u.Id == bookingId);
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
        }

        public void UpdateStripePaymentID(int bookingId, string sesstionId, string paymentIntentId)
        {
            var bookingFromDB = _db.Bookings.FirstOrDefault(u => u.Id == bookingId);
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
        }

    }
}