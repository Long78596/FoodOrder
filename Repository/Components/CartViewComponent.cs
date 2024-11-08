using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Repository.Components
{
    public class CartViewComponent:ViewComponent
    {
        private readonly DataContext _dataContext;
        public CartViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IViewComponentResult Invoke()
        {
            var giohang = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            return View(new CartItemViewModel
            {
                CartItems = giohang,
                Quantity = giohang.Sum(p => p.Quantity),
                GrandTotal = giohang.Sum(c => c.Total),
            });
        }
    }
}
