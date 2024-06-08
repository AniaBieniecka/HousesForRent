using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace HousesForRent.Web.ViewModels
{
    public class HomeVM
    {
        public List<HouseVM> HouseVMList { get; set; }
        [Display(Name ="Check-in date")]
        public DateOnly CheckInDate { get; set; }
        [Display(Name = "Check-out date")]
        public DateOnly? CheckOutDate { get; set; }
        [Display(Name = "Nights quantity")]
        public int NightsQty { get; set; }

        [Display(Name = "People quantity")]
        public int PeopleQty { get; set; }
        public IEnumerable<Amenity> AmenityList { get; set;}
    }
}
