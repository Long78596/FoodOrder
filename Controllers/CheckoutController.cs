using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Areas.Admin.Repository;
using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.Models.Order;
using FoodOrder.Models.ViewModels;
using FoodOrder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;

namespace FoodOrder.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly DataContext _datacontext;
        private readonly UserManager<AppUserModel> _userManager;
        private readonly IEmailSendercs _emailSender;
        private readonly INotyfService _notyfService;
        public CheckoutController(DataContext datacontext,UserManager<AppUserModel> userManager,IEmailSendercs emailSender,INotyfService notyfService)
        {
            _datacontext = datacontext;
            _userManager = userManager;
            _emailSender = emailSender;
            _notyfService = notyfService;
        }
        public async Task<IActionResult>Index() { 
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Lấy thông tin người dùng từ Identity
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            
            double totalPrice = cart.Sum(item => item.Quantity * item.Price);

            //// Tạo model cho view
            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = cart,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                TotalPrice = totalPrice
            };

            return View(checkoutViewModel);
        }

        public async Task<IActionResult> ConfirmPayment(CheckoutViewModel model)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = ordercode;


                var shippingPriceCookie = Request.Cookies["ShippingPrice"];
                double shippingPrice = 0;

                if (shippingPriceCookie != null)
                {
                    var shippingPriceJson = shippingPriceCookie;
                    shippingPrice = JsonConvert.DeserializeObject<double>(shippingPriceJson);
                }
                else
                {
                    shippingPrice = 0; 
                }

                var coupon_code = Request.Cookies["CouponTitle"];

                orderItem.Coupon = coupon_code;
                orderItem.ShippingCost = shippingPrice;
                orderItem.UserName = userEmail;
                orderItem.Phone = model.Phone;                
                orderItem.Address = model.Address;
                orderItem.OrderNotes = model.OrderNotes;
                orderItem.Status = 1;
                orderItem.Delivery_Date = DateTime.Now;
                orderItem.Delivery_Status = 0;
                orderItem.ShipperId=1;
                orderItem.CreateDate = DateTime.Now;
                _datacontext.Add(orderItem);
                _datacontext.SaveChanges();
                var orderId = orderItem.Id;
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                    foreach (var item in cartItems)
                {
                    var orderdetail = new OrderDetail();
                    orderdetail.OrderId = orderId;
                    //orderdetail.AppUser.UserName = userEmail;
                    orderdetail.OrderCode = ordercode;
                    orderdetail.Sold = item.Price;
                    orderdetail.FoodId= item.FoodId;
                    orderdetail.Quantity_Sold = item.Quantity;
                    var product = await _datacontext.Foods.AsNoTracking().FirstOrDefaultAsync(p => p.Id == item.FoodId);
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity;
                        product.Sold = (product.Sold ?? 0) + item.Quantity;

                        _datacontext.Update(product);
                        await _datacontext.SaveChangesAsync();
                    }
                    _datacontext.Update(product);

                    _datacontext.Add(orderdetail);
                    _datacontext.SaveChanges();

                }
                HttpContext.Session.Remove("Cart");
                var recevier = "longkolp16@gmail.com";
                var subject = "Đặt hàng hành công";
                var message = "Đặt hàng thành công , trải nghiệm dịch vụ nhé";
                await _emailSender.SendEmailAsync(recevier, subject, message);



                _notyfService.Success("Thanh toán thành công, vui lòng kiểm tra gmail của bạn!");
                return RedirectToAction("History", "Account");
            }
            return View();
        }


    }
}
