using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Shipper.Controllers
{

    [Area("Shipper")]
    //[Authorize]
  
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly INotyfService _notyfService;
        public HomeController(DataContext dataContext, INotyfService notyfService)
        {
            _dataContext = dataContext;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            var hoaDons = _dataContext.Orders
                                      
                                      .OrderByDescending(h => h.CreateDate)
                                      .ToList();

            var orderDetails = _dataContext.OrderDetails
                                           .Include(c => c.Food)
                                           .Include(ct => ct.Order)
                                           .Where(ct => hoaDons.Select(h => h.OrderCode).Contains(ct.OrderCode))
                                           .ToList();

            ViewBag.HoaDons = hoaDons;
            ViewBag.OrderDetails = orderDetails;

            return View();
        }
        
        [HttpPost]
        public IActionResult Orderprocess(string ordercode)
        {
            var hoaDon = _dataContext.Orders.FirstOrDefault(h => h.OrderCode == ordercode);
            if (hoaDon != null)
            {
                hoaDon.Delivery_Status = 1;
                _dataContext.SaveChanges();
                _notyfService.Success("Xác nhận giao hàng thành công!");
                return RedirectToAction("Index");

            }
            else
            {
                _notyfService.Error("Đơn hàng không tồn tại!");
            }
            _notyfService.Success("Xác nhận giao hàng thành công!");


            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult ConfirmDelivery(string ordercode)
        {
            
            var order = _dataContext.Orders.FirstOrDefault(o => o.OrderCode == ordercode);
            if (order != null)
            {
                order.Delivery_Status = 2;
                _dataContext.SaveChanges();
            }

            return RedirectToAction("Index");
        
    }

        [HttpPost]
        public IActionResult CancelOrder(string ordercode)
        {
            var order = _dataContext.Orders.FirstOrDefault(o => o.OrderCode == ordercode);
            if (order != null)
            {
                order.Delivery_Status = 3;
                _dataContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Order()
        {
            var hoaDons = _dataContext.Orders.Where(d => d.Delivery_Status == 1).OrderByDescending(h => h.CreateDate).ToList();

            var orderDetails = _dataContext.OrderDetails
                                .Include(c => c.Food)
                                .Include(ct => ct.Order)
                                .Where(ct => hoaDons.Select(h => h.OrderCode).Contains(ct.OrderCode))
                                .ToList();

            ViewBag.HoaDons = hoaDons;
            ViewBag.OrderDetails = orderDetails;

            return View();
        }
    }

    }
