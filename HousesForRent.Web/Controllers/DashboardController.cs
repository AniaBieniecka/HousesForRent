using Microsoft.AspNetCore.Mvc;

namespace HousesForRent.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
