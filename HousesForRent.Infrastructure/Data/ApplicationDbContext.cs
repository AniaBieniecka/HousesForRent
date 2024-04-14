using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Infrastructure.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<House> Houses { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<HouseAmenity> HouseAmenities { get; set; }
    }
}
