using HousesForRent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Services.Interface
{
    public interface IHouseAmenityService
    {
        IEnumerable<HouseAmenity> GetAllHouseAmenities(int? houseId);
        HouseAmenity GetHouseAmenity(int houseId, int amenityId);
        void CreateHouseAmenity(HouseAmenity houseAmenity);
        void UpdateHouseAmenity(HouseAmenity houseAmenity);
        bool DeleteHouseAmenity(int id);

    }
}
