using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShipperController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly INotyfService _notyfService;
        public ShipperController(DataContext dataContext, INotyfService notyfService)
        {
            _dataContext = dataContext;
            _notyfService = notyfService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var shipper = _dataContext.Orders.Include(c => c.Shipper).OrderByDescending(x => x.Id).ToList();
            return View(shipper);
        }
    }
  }
