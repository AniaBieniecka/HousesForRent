using HousesForRent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Common.Interfaces
{
    public interface IHouseRepository: IRepository<House>
    {
        void Update(House house);
        void Save();
        public IEnumerable<House> GetAllHouses();
        public House GetHouse();


    }
}
