using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace HousesForRent.Web.ViewModels
{
    public class BookingVM
    {
        public Booking Booking { get; set; }
        public HouseVM HouseVM { get; set; }

    }
}
