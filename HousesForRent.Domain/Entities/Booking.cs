using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int HouseId { get; set; }
        [ForeignKey("HouseId")]
        public House House { get; set; }
        [Required]
        [Display(Name ="Name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string UserEmail { get; set; }
        public string? Phone {  get; set; }

        [Required]
        [Display(Name = "Nights quantity")]

        public int NightsQty { get; set; }
        [Required]
        public double Cost { get; set; }
        public string? Status { get; set; }
        [Required]
        [Display(Name = "Booking date")]
        public DateTime BookingDate { get; set; }
        [Required]
        [Display(Name = "Check-in date")]
        public DateOnly CheckInDate { get; set; }
        [Required]
        [Display(Name = "Check-out date")]
        public DateOnly CheckOutDate { get; set; }
        public bool IsPaymentSuccessful { get; set; } = false;
        [Display(Name = "Payment date")]
        public DateTime PaymentDate { get; set; }
        [Display(Name = "Stripe session Id")]
        public string? StripeSessionId { get; set; }
        [Display(Name = "Stripe paymen intent Id")]
        public string? StripePaymentIntentId { get; set; }
        [Display(Name = "Actual check-in date")]
        public DateTime ActualCheckInDate { get; set; }
        [Display(Name = "Actual check-out date")]
        public DateTime ActualCheckOutDate { get; set; }

    }
}
