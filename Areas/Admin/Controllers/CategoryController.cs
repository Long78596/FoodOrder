using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
