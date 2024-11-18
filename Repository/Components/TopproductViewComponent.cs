using FoodOrder.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Repository.Components
{
    public class TopproductViewComponent:ViewComponent
    {
        private readonly DataContext _dataContext;
        public TopproductViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var foodnew = _dataContext.Foods.Where(x => x.Status == true).Include(d => d.Category).Skip(3).Take(3).ToList();
            var Topfood = _dataContext.Foods.Where(x => x.Status == true).Include(d => d.Category).Skip(6).Take(3).ToList();
            ViewBag.foodnew = foodnew;
            ViewBag.Topfood = Topfood;


            return View(Topfood);
        }
    }
}
