using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Common.Utility;
using HousesForRent.Domain.Entities;
using HousesForRent.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Drawing;
using System.ComponentModel;
using System.Drawing;
using System.Security.Claims;

namespace HousesForRent.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BookingController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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

            //checking availability:
            var bookings = _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusPending ||
            u.Status == SD.StatusApproved || u.Status == SD.StatusCheckedIn || u.HouseId == booking.HouseId).ToList();
            house.IsBooked = SD.isHouseBooked(house.Id, booking.CheckInDate, booking.NightsQty, bookings);
            if(house.IsBooked)
            {
                TempData["error"] = "House has just been sold out";
                return RedirectToAction(nameof(ProcessBooking), new
                {
                    houseId=booking.HouseId,
                    checkInDate = booking.CheckInDate,
                    nightsQty = booking.NightsQty
                });
            }

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
        [Authorize]
        [HttpPost]
        public IActionResult CreateInvoice (int bookingId)
        {
            string basePath = _webHostEnvironment.WebRootPath;

            WordDocument document = new WordDocument();
            string dataPath = basePath + @"/exports/BookingDetails.docx";
            using FileStream fileStream = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            document.Open(fileStream, FormatType.Automatic);

            Booking booking = _unitOfWork.Booking.Get(u => u.Id == bookingId, includeProperties: "User,House");
            TextSelection textSelection = document.Find("xx_customer_name", false, true);
            WTextRange textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.UserName;

            textSelection = document.Find("XX_BOOKING_NUMBER ", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text ="BOOKING ID: " + booking.Id.ToString();

            textSelection = document.Find("XX_BOOKING_DATE ", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = "BOOKING DATE: " + booking.BookingDate.ToShortDateString();

            textSelection = document.Find("xx_customer_phone", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.Phone;

            textSelection = document.Find("xx_customer_email", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.UserEmail;

            textSelection = document.Find("xx_payment_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.PaymentDate.ToShortDateString();

            textSelection = document.Find("xx_checkin_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.CheckInDate.ToShortDateString();

            textSelection = document.Find("xx_checkout_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.CheckOutDate.ToShortDateString();

            textSelection = document.Find("xx_booking_cost", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.Cost.ToString("c");

            using DocIORenderer renderer = new();
            MemoryStream stream = new();
            document.Save(stream, FormatType.Docx);
            stream.Position = 0;

            return File(stream, "application/docx", "BookingDetails.docx");
        }

        [HttpPost]
        [Authorize(Roles =SD.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCheckedIn);
            _unitOfWork.Booking.Save();
            TempData["Success"] = "Booking updated successfully";
            return RedirectToAction(nameof(BookingDetails), new {bookingId = booking.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCompleted);
            _unitOfWork.Booking.Save();
            TempData["Success"] = "Booking completed successfully";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Cancel(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCancelled);
            _unitOfWork.Booking.Save();
            TempData["Success"] = "Booking cancelled successfully";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });

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
