using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Reflection.Metadata.Ecma335;

namespace HousesForRent.Web.ViewModels
{
    public class HouseVM
    {
        public House? House { get; set; }
        [ValidateNever]
        public List<Amenity> AmenityList { get; set; }
        [ValidateNever]
        public List<int> HouseAmenitiesIdList { get; set; }
    }
}
