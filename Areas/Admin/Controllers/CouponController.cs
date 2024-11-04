using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly INotyfService _notyfService;
        public CouponController(DataContext dataContext, INotyfService notyfService)
        {
            _dataContext = dataContext;
            _notyfService = notyfService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var coupon = _dataContext.Coupons.OrderByDescending(c => c.Id).ToList();
            return View(coupon);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponModel coupon)
        {
            var coupon_name = await _dataContext.Coupons.FirstOrDefaultAsync(p => p.Name == coupon.Name);
            if (coupon_name != null)
            {
                _notyfService.Error("Mã đã tồn tại!");
                return View(coupon);
            }
            _dataContext.Add(coupon);
            await _dataContext.SaveChangesAsync();
            _notyfService.Success("Thêm thành công!");
            return RedirectToAction("Index");
        }

    }
}
