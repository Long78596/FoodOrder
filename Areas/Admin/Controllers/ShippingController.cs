using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("Admin/Shipping")]
    [Authorize(Roles = "Admin")]
    public class ShippingController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly INotyfService _notyfService;
        public ShippingController(DataContext dataContext,INotyfService notyfService)
        {
            _dataContext = dataContext;
            _notyfService = notyfService;

        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var shippinglist = await _dataContext.Shippings.ToListAsync();
            ViewBag.Shippings = shippinglist;
            return View();
        }
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Route("StoreShipping")]
        public async Task<IActionResult> AddShipping(ShippingModel shippingModel, string phuong, string quan, string tinh, double price)
        {
            shippingModel.City = tinh;
            shippingModel.Dictric = quan;
            shippingModel.Ward = phuong;
            shippingModel.Price = price;
            try
            {
                var existingShipping = await _dataContext.Shippings.AnyAsync(x => x.City == tinh && x.Dictric == quan && x.Ward == phuong);
                if (existingShipping)
                {
                    return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp" });
                }
                _dataContext.Shippings.Add(shippingModel);
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm shipping thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occured while adding shipping");
            }
        }
        [HttpPost]       
        
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                ShippingModel shipping = await _dataContext.Shippings.FindAsync(Id);

                if (shipping == null)
                {
                    _notyfService.Warning("Không tìm thấy đơn hàng để xóa.");
                    return RedirectToAction("Index", "Shipping");
                }

                _dataContext.Shippings.Remove(shipping);
                await _dataContext.SaveChangesAsync();

                _notyfService.Success("Xóa đơn hàng thành công!");
                return RedirectToAction("Index", "Shipping");
            }
            catch (Exception ex)
            {
                _notyfService.Error("Đã xảy ra lỗi khi xóa đơn hàng: " + ex.Message);
                return RedirectToAction("Index", "Shipping");
            }
        }


    }
}
