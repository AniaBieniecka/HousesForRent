using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Domain.Entities
{
    public class HouseAmenity
    {
        public int Id { get; set; }

        [ForeignKey("House")]
        public int HouseId { get; set; }
        [ValidateNever]
        public House house { get; set; }

        [ForeignKey("Amenity")]
        public int AmenityId { get; set; }
        [ValidateNever]
        public Amenity amenity { get; set; }

    }
}
