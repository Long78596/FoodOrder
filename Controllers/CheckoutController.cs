using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.Models.ViewModels;
using FoodOrder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrder.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly DataContext _datacontext;
        private readonly UserManager<AppUserModel> _userManager;
        public CheckoutController(DataContext datacontext,UserManager<AppUserModel> userManager)
        {
            _datacontext = datacontext;
            _userManager = userManager;
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
       
        public async Task<IActionResult> Index1()
        {
            return View();
        }
            
        
    }
}
