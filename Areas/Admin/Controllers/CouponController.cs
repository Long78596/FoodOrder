using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var coupon = await _dataContext.Coupons.FindAsync(Id);
            if (coupon == null)
            {
                return NotFound(); // Return 404 if not found
            }
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CouponModel couponmodel)
        {

            var exists_coupon = await _dataContext.Coupons.FindAsync(couponmodel.Id);
            var coupon_name = await _dataContext.Coupons.FirstOrDefaultAsync(p => p.Name == couponmodel.Name);

            if (coupon_name != null && coupon_name.Id != couponmodel.Id)
            {
                _notyfService.Error("Danh mục  đã tồn tại!");
                return View(coupon_name);
            }
            if (exists_coupon == null)
            {
                return NotFound();
            }
            exists_coupon.Name = couponmodel.Name;
            exists_coupon.Description = couponmodel.Description;
            exists_coupon.Quantity = couponmodel.Quantity;
            exists_coupon.Status = couponmodel.Status;
            _dataContext.Update(exists_coupon);
            await _dataContext.SaveChangesAsync();
            _notyfService.Success("Cập nhật  thành công!");
            return RedirectToAction("Index");

        }


        public async Task<IActionResult> Delete(int Id)
        {
            var coupon = await _dataContext.Coupons.FindAsync(Id);
            if (coupon == null)
            {
                return NotFound();
            }
            _dataContext.Coupons.Remove(coupon);
            await _dataContext.SaveChangesAsync();
            _notyfService.Success("Xóa  thành công!");
            return RedirectToAction("Index");
        }

    }
}
