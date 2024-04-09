using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IHouseRepository House {  get; }
        IAmenityRepository Amenity { get; }
        IHouseAmenityRepository HouseAmenity { get; }
    }
}
