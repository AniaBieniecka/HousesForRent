using Microsoft.Extensions.ObjectPool;

namespace HousesForRent.Web.ViewModels
{
    public class BarChartDTO
    {
        public List<BarChartData> Series { get; set; }
        public string[] Categories { get; set; }
    }

    public class BarChartData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double[] Data { get; set; }
        public string Unit { get; set; }
    }
}
