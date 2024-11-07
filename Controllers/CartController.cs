using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.Models.ViewModels;
using FoodOrder.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Controllers
{
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
            CartItemViewModel cartVM = new()
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Quantity * x.Price)
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
            return RedirectToAction("Cart");

        }
        public async Task<IActionResult> Increase(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel giohangVM = cart.Where(c => c.FoodId == Id).FirstOrDefault();
            if (giohangVM.Quantity >= 1)
            {
                ++giohangVM.Quantity;
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
                HttpContext.Session.SetJson("cart", cart);
            }
            _notyfService.Success("Tăng số lượng thành công!");


            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p => p.FoodId == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("GioHang");
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
    }
}
