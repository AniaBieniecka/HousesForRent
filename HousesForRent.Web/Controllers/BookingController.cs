using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HousesForRent.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitoOfWork;
        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitoOfWork = unitOfWork;
        }
        public IActionResult ProcessBooking(int houseId, int nightsQty, string checkInDate)
        {

            
            DateOnly.TryParseExact(checkInDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateOnly checkInDateParsed);

            Booking booking = new()
            {
                HouseId = houseId,
                House = _unitoOfWork.House.GetHouse(),
                CheckInDate = checkInDateParsed,
                CheckOutDate = checkInDateParsed.AddDays(nightsQty),
                NightsQty = nightsQty,
            };
            booking.Cost = booking.House.Price * nightsQty;

            return View(booking);
        }
    }
}
