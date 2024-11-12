using FoodOrder.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DashboardController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {

            var count_food = _dataContext.Foods.Count();
            var count_order = _dataContext.Orders.Count();
            var count_category = _dataContext.Categories.Count();
            var count_user = _dataContext.Users.Count();
            ViewBag.CountFood = count_food;
            ViewBag.CountOrder = count_order;
            ViewBag.CountCategory = count_category;
            ViewBag.CountUser = count_user;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetChartData()
        {
            var data = _dataContext.Statisticals.Select(s => new
            {
                date = s.DateCreated.ToString("yyyy-MM-dd"),
                sold = s.Sold_Order,
                quantity = s.Quantity_Sold,
                revenue = s.Revenue,
                profit = s.Profit,
            }).ToList();
            return Json(data);

        }
        [HttpPost]
        [Route("GetChartDataBySelect")]
        public IActionResult GetChartDataBySelect(DateTime startDate, DateTime endDate)
        {
            var data = _dataContext.Statisticals.Where(s => s.DateCreated >= startDate && s.DateCreated <= endDate)
                .Select(s => new { 
                    date=s.DateCreated.ToString("yyyy-MM-dd"),
                    slod=s.Sold_Order,
                    quantity=s.Quantity_Sold,
                    revenue=s.Revenue,
                    profit=s.Profit,

                
                }).ToList();
            return Json(data);

        }
        [HttpPost]
        [Route("FilterData")]
        public IActionResult FilterData(DateTime? formDate, DateTime? toDate)
        {
            var query = _dataContext.Statisticals.AsQueryable();
            if (formDate.HasValue)
            {
                query = query.Where(s => s.DateCreated >= formDate);
            }
            if (toDate.HasValue)
            {
                query = query.Where(s => s.DateCreated <= toDate);
            }
             var date=query.Select(s => new {
				 date = s.DateCreated.ToString("yyyy-MM-dd"),
				 slod = s.Sold_Order,
				 quantity = s.Quantity_Sold,
				 revenue = s.Revenue,
				 profit = s.Profit,


			 }).ToList();
            return Json(date);
		}
        
    
    }
}
