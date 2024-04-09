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
    public class HouseAmenityRepository : Repository<HouseAmenity>, IHouseAmenityRepository
    {
        private readonly ApplicationDbContext _db;
        public HouseAmenityRepository(ApplicationDbContext db): base(db) { 
            _db = db; }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(HouseAmenity houseAmenity)
        {
            _db.Update(houseAmenity);
        }
    }
}
