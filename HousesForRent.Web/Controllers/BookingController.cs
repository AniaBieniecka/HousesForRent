using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;
using System.Security.Claims;

namespace HousesForRent.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult ProcessBooking(int houseId, int nightsQty, string checkInDate)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            DateOnly.TryParseExact(checkInDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateOnly checkInDateParsed);

            Booking booking = new()
            {
                HouseId = houseId,
                House = _unitOfWork.House.GetHouse(),
                CheckInDate = checkInDateParsed,
                CheckOutDate = checkInDateParsed.AddDays(nightsQty),
                NightsQty = nightsQty,
                UserId = userId,
                UserName = user.Name,
                UserEmail = user.Email,
                Phone = user.PhoneNumber
            };
            booking.Cost = booking.House.Price * nightsQty;

            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProcessBooking(Booking booking)
        {
            var house = _unitOfWork.House.Get(u => u.Id == booking.HouseId);

            booking.Cost = house.Price * booking.NightsQty;
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;
            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Booking.Save();


            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                Mode = "payment",
                SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"booking/ProcessBooking?houseId={booking.HouseId}&checkInDate={booking.CheckInDate}&nights={booking.NightsQty}",
            };

            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.Cost * 100),
                    Currency = "pln",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = house.Name
                    },
                },
                Quantity = 1,
            });

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.Booking.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Booking.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            Booking bookingFromDB = _unitOfWork.Booking.Get(u=>u.Id == bookingId, includeProperties: "User,House");
            if (bookingFromDB.Status == SD.StatusPending)
            {
                //confirming if payment was successful
                var service = new SessionService();
                Session session = service.Get(bookingFromDB.StripeSessionId);

                if (session.PaymentStatus == "paid")
                {
                    _unitOfWork.Booking.UpdateStatus(bookingFromDB.Id, SD.StatusApproved);
                    _unitOfWork.Booking.UpdateStripePaymentID(bookingFromDB.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Booking.Save();
                }
            }
            return View(bookingId);
        }

        [Authorize]
        public  IActionResult BookingDetails (int bookingId)
        {
            Booking bookingFromDB = _unitOfWork.Booking.Get(u => u.Id == bookingId, includeProperties: "User,House");
            return View(bookingFromDB);
        }

        #region API CALLS
        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> objBookings;

            if (User.IsInRole(SD.Role_Admin)){
                objBookings = _unitOfWork.Booking.GetAll(includeProperties: "User,House");
            } else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objBookings = _unitOfWork.Booking.GetAll(u => u.UserId == userId, includeProperties:"User,House");
            }
            if (!string.IsNullOrEmpty(status))
            {
                objBookings = objBookings.Where(u=>u.Status == status);
            }
            return Json(new { data=objBookings });
        }

        #endregion
    }
}
