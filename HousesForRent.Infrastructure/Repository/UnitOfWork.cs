using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Domain.Entities;
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
        public IHouseAmenityRepository HouseAmenity { get; private set;}
        public IBookingRepository Booking { get; private set;}
        public UnitOfWork(ApplicationDbContext db)
    {
            _db = db;
            House = new HouseRepository(_db);
            Amenity = new AmenityRepository(_db);
            HouseAmenity = new HouseAmenityRepository(_db);
            Booking = new BookingRepository(_db);
        }
    }
}
