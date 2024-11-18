using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _context;
        private readonly INotyfService _notyfService;
        public OrderController(DataContext context,INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _context.Orders.OrderByDescending(p => p.Id).ToListAsync());
        }
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            var detailsOrder = await _context.OrderDetails
                                     .Include(o => o.Order)  
                                     .Include(o => o.Food)   
                                     .Where(o => o.OrderCode == ordercode)
                                     .ToListAsync();
            var order = _context.Orders.Where(o => o.OrderCode == ordercode).First();
            ViewBag.Status = order.Status;
            return View(detailsOrder);
        }
        [HttpPost]
        [Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            _context.Update(order);
            if(status == 0)
            {
                var detailsOrder = await _context.OrderDetails.Include(od => od.Food).Where(od => od.OrderCode == ordercode)
                    .Select(od => new
                    {
                        od.Quantity_Sold,
                        od.Food.Price,
                        od.Food.CapitalPrice,
                    }).ToListAsync();
                var staticialModel = await _context.Statisticals.FirstOrDefaultAsync(s => s.DateCreated.Date == order.CreateDate.Date);
                if(staticialModel != null)
                {
                    foreach(var orderDetail in detailsOrder)
                    {
                        staticialModel.Quantity_Sold += 1;
                        staticialModel.Sold_Order += orderDetail.Quantity_Sold;
                        staticialModel.Revenue += Convert.ToInt32(orderDetail.Quantity_Sold * orderDetail.Price);
                        staticialModel.Profit += Convert.ToInt32(orderDetail.Price - orderDetail.CapitalPrice);
                    }
                    _context.Update(staticialModel);

                }
                else
                {
                    int new_quantity = 0;
                    int new_sold = 0;
                    int new_profit = 0 ;
                    foreach (var orderDetail in detailsOrder)
                    {
                        new_quantity += 1;
                        new_sold += orderDetail.Quantity_Sold;
                        new_profit += Convert.ToInt32(orderDetail.Price - orderDetail.CapitalPrice);
                        staticialModel = new Models.StatisticalModel
                        {
                           DateCreated=order.CreateDate,
                           Quantity_Sold= new_quantity,
                           Sold_Order=new_sold,
                           Revenue= Convert.ToInt32(orderDetail.Quantity_Sold * orderDetail.Price),
                           Profit = new_profit
                        };
                    }
                    _context.Add(staticialModel);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Đã cập nhật thành công!" });
            }catch(Exception ex)
            {
                return StatusCode(500, "Lỗi");
            }
       
        }
        public async Task<IActionResult> Delete(string Id)
        {
            var orderDetails = await _context.OrderDetails
                                    .Include(od => od.Order)
                                    .Where(od => od.OrderId == Id)
                                    .ToListAsync();

            if (orderDetails.Any())
            {
                _context.OrderDetails.RemoveRange(orderDetails);

                var order = orderDetails.First().Order;
                if (order != null)
                {
                    _context.Orders.Remove(order);
                }

                await _context.SaveChangesAsync();
                _notyfService.Success("Xóa đơn hàng thành công!");
            }
            else
            {
                _notyfService.Warning("Đơn hàng không tồn tại!");
            }

            return RedirectToAction("Index");
        }

    }
}
