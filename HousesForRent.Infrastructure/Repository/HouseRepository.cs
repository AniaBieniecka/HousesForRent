using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Domain.Entities;
using HousesForRent.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Infrastructure.Repository
{
    public class HouseRepository : Repository<House>, IHouseRepository
    {
        private readonly ApplicationDbContext _db;
        public HouseRepository(ApplicationDbContext db): base(db) { 
            _db = db; }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(House house)
        {
            _db.Update(house);
        }
        public IEnumerable<House> GetAllHouses()
        {
            IQueryable<House> query = dbSet;
            query = query.Include(u => u.houseAmenities).ThenInclude(e=>e.amenity);

            return query.ToList();
        }
    }
}
