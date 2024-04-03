using System;
using System.Collections.Generic;
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
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public int Occupancy { get; set; }        
        public int RoomsQuantity { get; set; }
        public int SingleBedQuantity { get; set; }
        public int DoubleBedQuantity { get; set; }
        public int Area {  get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set;}

    }
}
