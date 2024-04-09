using HousesForRent.Domain.Entities;
using System.Reflection.Metadata.Ecma335;

namespace HousesForRent.Web.ViewModels
{
    public class HouseVM
    {
        public House? House { get; set; }
        public List<Amenity> AmenityList { get; set; }
    }
}
