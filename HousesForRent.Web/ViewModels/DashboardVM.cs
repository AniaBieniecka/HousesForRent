using HousesForRent.Domain.Entities;

namespace HousesForRent.Web.ViewModels
{
    public class DashboardVM
    {
        public List<BookingDto>? lastBookings;
        public List<HouseDto>? popularHouses;

        public class BookingDto 
        {
            public DateOnly bookingDate { get; set; }
            public string houseName { get; set; }
            public double cost { get; set; }
        }

        public class HouseDto
        {
            public string houseName { get; set; }
            public double bookingQty { get; set; }
        }

    }
}
