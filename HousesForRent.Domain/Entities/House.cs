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
    public class House
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string Location { get; set; }
        [Display(Name = "Price per night")]
        [Range(0, double.MaxValue)]

        public double Price { get; set; }
        [Display(Name = "Discount price per night")]
        [Range(0, double.MaxValue)]
        public double DiscountPrice { get; set; }
        [Display(Name = "Max. occupancy")]
        public int Occupancy { get; set; }
        [Display(Name = "Number of rooms")]
        [Range(0, int.MaxValue)]

        public int RoomsQuantity { get; set; }
        [Display(Name = "Number of single beds")]
        [Range(0, int.MaxValue)]
        public int SingleBedQuantity { get; set; }
        [Display(Name = "Number of double beds")]
        [Range(0, int.MaxValue)]
        public int DoubleBedQuantity { get; set; }
        [Display(Name = "Area in square meters")]
        [Range(0, int.MaxValue)]
        public int Area { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        [Display(Name = "Image Url")]
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [NotMapped]
        public bool IsBooked { get; set; } = false;

        [ValidateNever]
        public IEnumerable<HouseAmenity> houseAmenities { get; set; }

    }
}
