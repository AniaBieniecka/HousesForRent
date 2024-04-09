using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IHouseRepository House {get; private set;}
        public IAmenityRepository Amenity { get; private set;}
        public UnitOfWork(ApplicationDbContext db)
    {
            _db = db;
            House = new HouseRepository(_db);
            Amenity = new AmenityRepository(_db);
        }
    }
}
