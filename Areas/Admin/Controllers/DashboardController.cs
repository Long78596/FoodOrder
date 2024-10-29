using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
