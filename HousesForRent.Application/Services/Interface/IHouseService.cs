using HousesForRent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Services.Interface
{
    public interface IHouseService
    {
        IEnumerable<House> GetAllHouses(int? peopleQty=null);
        House GetHouse(int id);
        void CreateHouse(House house);
        void UpdateHouse(House house);
        bool DeleteHouse(int id);
    }
}
