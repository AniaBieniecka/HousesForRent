namespace HousesForRent.Web.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<string> Roles { get; set; }
        public int BookingsQty { get; set; }

    }
}
