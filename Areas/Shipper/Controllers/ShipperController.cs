using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Areas.Shipper.Controllers
{
    public class ShipperController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
