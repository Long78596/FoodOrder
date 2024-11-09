using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.Models.ViewModels;
using FoodOrder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FoodOrder.Controllers
{

    [Authorize]
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly INotyfService _notyfService;
        public CartController(DataContext context, INotyfService notyfService)
        {
            _dataContext = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var coupon_code = Request.Cookies["CouponTitle"];
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            double shippingPrice = 0; 

            if (!string.IsNullOrEmpty(shippingPriceCookie))
            {
                shippingPrice = JsonConvert.DeserializeObject<double>(shippingPriceCookie);
            }

            CartItemViewModel cartVM = new()
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Quantity * x.Price),
                CouponCode=coupon_code,
                ShippingCost=shippingPrice,
            };
            return View(cartVM);
        }
        public async Task<IActionResult> AddToCart(int Id, int quantity = 1)
        {
            FoodModel foods = await _dataContext.Foods.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel GiohangItems = cart.Where(c => c.FoodId == Id).FirstOrDefault();
            if (GiohangItems == null)
            {
                CartItemModel gioHangItem = new  CartItemModel(foods)
                {
                    Quantity= quantity
                };


                cart.Add(gioHangItem);

            }
            else
            {
                GiohangItems.Quantity += quantity;
            }
            _notyfService.Success("thêm  thành công!");
            HttpContext.Session.SetJson("Cart", cart);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UpdateCart(int FoodId, int quantity)
        {
            FoodModel foodItem = await _dataContext.Foods.FindAsync(FoodId);
            if (foodItem == null)
            {
                _notyfService.Error("Sản phẩm không tồn tại.");
                return RedirectToAction("Index");
            }

            if (quantity > foodItem.Quantity)
            {
                _notyfService.Error($"Số lượng yêu cầu vượt quá số lượng hiện có. Hiện chỉ còn {foodItem.Quantity} có thể đáp ứng.");
                return RedirectToAction("Index");
            }

            // Lấy giỏ hàng từ session
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            CartItemModel giohangVM = cart.FirstOrDefault(c => c.FoodId == FoodId);

            if (giohangVM != null)
            {
                
                giohangVM.Quantity += quantity;

                
                HttpContext.Session.SetJson("Cart", cart);
                _notyfService.Success("Cập nhật giỏ hàng thành công!");
            }
            else
            {
                
                _notyfService.Error("Sản phẩm không có trong giỏ hàng.");
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel giohangVM = cart.Where(c => c.FoodId == Id).FirstOrDefault();
            if (giohangVM.Quantity > 1)
            {
                --giohangVM.Quantity;
            }
            else
            {
                cart.RemoveAll(c => c.FoodId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            _notyfService.Success("Giam số lượng thành công!");
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Increase(int Id)
        {
            FoodModel product = await _dataContext.Foods.Where(p => p.Id == Id).FirstOrDefaultAsync();

            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.FirstOrDefault(x => x.FoodId == Id);

            if (product == null || cartItem == null)
            {
                _notyfService.Error("Sản phẩm không tồn tại trong giỏ hàng hoặc không có sẵn.");
                return RedirectToAction("Index");
            }

            if (cartItem.Quantity + 1 > product.Quantity)
            {
                _notyfService.Error($"Số lượng yêu cầu vượt quá số lượng hiện có. Hiện chỉ còn {product.Quantity} sản phẩm có thể đáp ứng.");
            }
            else
            {
                cartItem.Quantity++;
                _notyfService.Success("Tăng số lượng sản phẩm thành công!");
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p => p.FoodId == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            _notyfService.Success("Xóa món ăn thành công!");
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> XoaHet()
        {
            HttpContext.Session.Remove("Cart");
            _notyfService.Success("Xóa giỏ hàng thành công!");
            return RedirectToAction("Cart");
        }
        [HttpPost]
        [Route("Cart/GetCoupon")]
        public async Task<IActionResult> GetCoupon(CouponModel couponModel, string coupon_value)
        {
            var validCoupon = await _dataContext.Coupons
                .FirstOrDefaultAsync(x => x.Name == coupon_value && x.Quantity >= 1);
            if (validCoupon == null)
            {
                return Ok(new { success = false, message = "Mã giảm giá đã hết hạn hoặc đã hết số lượng" });
            }
            string couponTitle = validCoupon.Name + " - " + validCoupon?.Description;

            if (couponTitle != null)
            {
                TimeSpan remainingTime = validCoupon.DataExpired - DateTime.Now;
                int daysRemaining = remainingTime.Days;

                if (daysRemaining >= 0)
                {
                    try
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                            Secure = true,
                            SameSite = SameSiteMode.Strict // Kiểm tra tính tương thích trình duyệt
                        };

                        Response.Cookies.Append("CouponTitle", couponTitle, cookieOptions);
                        return Ok(new { success = true, message = "Coupon applied successfully" });
                    }
                    catch(Exception ex)
                    {
                        return Ok(new { success = false, message = "Coupon applied falied" });
                    }
                }
                else
                {
                    return Ok(new { success = false, message = "Coupon has expried" });
                }
            }
            else
            {
                return Ok(new { success = false, message = "Coupon has not exists" });
            }
        

            return Json(new { CouponTitle= couponTitle });
        }
        [HttpPost]
        [Route("Cart/GetShipping")]
        public async Task<IActionResult> GetShipping(ShippingModel shipping, string quan , string phuong, string tinh)
        {
            var existingShipping = await _dataContext.Shippings.FirstOrDefaultAsync(x => x.City == tinh && x.Dictric == quan && x.Ward == phuong);

            double shippingPrice = 0;
            if(existingShipping != null)
            {
                shippingPrice = existingShipping.Price;
            }
            else
            {
                shippingPrice = 15000;
            }
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure=true,
                };
                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);

            }
            catch(Exception ex)
            {
                Console.WriteLine("Lỗi");
            }
            return Json(new { shippingPrice });

        }
       
        
        
    }
}
