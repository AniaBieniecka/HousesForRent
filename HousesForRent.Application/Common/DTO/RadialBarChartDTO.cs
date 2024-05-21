namespace HousesForRent.Web.ViewModels
{
    public class RadialBarChartDTO
    {
        public decimal TotalCount { get; set; }
        public decimal CountCurrentMonth { get; set; }
        public int MonthlyChange { get; set; }
        public int[] Series { get; set; }
    }
}
