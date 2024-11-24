using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Areas.Admin.Repository;
using FoodOrder.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Shipper.Controllers
{

    [Area("Shipper")]
    [Authorize]
  
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly INotyfService _notyfService;
        private readonly IEmailSendercs _emailSender;
        public HomeController(DataContext dataContext, INotyfService notyfService, IEmailSendercs emailSender)
        {
            _dataContext = dataContext;
            _notyfService = notyfService;
            _emailSender = emailSender;
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
        public async Task<IActionResult> OrderProcess(string ordercode)
        {
            var hoaDon = _dataContext.Orders.FirstOrDefault(h => h.OrderCode == ordercode);
            if (hoaDon != null)
            {
                hoaDon.Delivery_Status = 1;
                await _dataContext.SaveChangesAsync();

              
                var customer = _dataContext.Shippers.FirstOrDefault(c => c.Id == hoaDon.ShipperId);
                if (customer != null)
                {
                    await _emailSender.SendEmailAsync(
                        customer.Email, 
                        "Đơn hàng đã đã được nhận đơn ",
                        "Đơn hàng của bạn đang giao đến khách hàng!. Cảm ơn bạn đã nhận đơn!"
                    );
                }

                TempData["success"] = "Xác nhận đơn hàng thành công,vui lòng kiểm gmail!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Đơn hàng không tồn tại!";
            }

            return RedirectToAction("Index");
        }

        


        [HttpPost]
        public async Task<IActionResult> ConfirmDelivery(string ordercode)
        {
            
            var order = _dataContext.Orders.FirstOrDefault(o => o.OrderCode == ordercode);
            if (order != null)
            {
                order.Delivery_Status = 2;
                _dataContext.SaveChanges();




                var shipper = _dataContext.Shippers.FirstOrDefault(c => c.Id == order.ShipperId);
                if (shipper != null)
                {
                    await _emailSender.SendEmailAsync(
                        shipper.Email,
                        "Đơn hàng đã bạn đã được giao! ",
                        "Đơn hàng của bạn đang giao đến khách hàng!. Cảm ơn bạn đã hoàn thành!"
                    );
                }

                TempData["success"] = "Xác nhận giao hàng thành công, vui lòng kiểm gmail! ";
            }

            return RedirectToAction("Index");
        
    }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(string ordercode)
        {
            var order = _dataContext.Orders.FirstOrDefault(o => o.OrderCode == ordercode);
            if (order != null)
            {
                order.Delivery_Status = 3;
                _dataContext.SaveChanges();
                var shipper = _dataContext.Shippers.FirstOrDefault(c => c.Id == order.ShipperId);
                if (shipper != null)
                {
                    await _emailSender.SendEmailAsync(
                        shipper.Email,
                        "Đơn hàng đã bạn đã bị hủy đơn! ",
                        "Đơn hàng của bạn đã hủy!. Cảm ơn bạn đã hoàn thành!"
                    );
                }
                TempData["success"] = "Xác nhận đơn hàng đã hủy,vui lòng kiểm gmail! ";
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
