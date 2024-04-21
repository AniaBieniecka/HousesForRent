using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace HousesForRent.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitoOfWork;
        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitoOfWork = unitOfWork;
        }
        [Authorize]
        public IActionResult ProcessBooking(int houseId, int nightsQty, string checkInDate)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _unitoOfWork.ApplicationUser.Get(u=>u.Id == userId);
            
            DateOnly.TryParseExact(checkInDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateOnly checkInDateParsed);

            Booking booking = new()
            {
                HouseId = houseId,
                House = _unitoOfWork.House.GetHouse(),
                CheckInDate = checkInDateParsed,
                CheckOutDate = checkInDateParsed.AddDays(nightsQty),
                NightsQty = nightsQty,
                UserName = user.Name,
                UserEmail = user.Email,
                Phone = user.PhoneNumber
            };
            booking.Cost = booking.House.Price * nightsQty;

            return View(booking);
        }
    }
}
