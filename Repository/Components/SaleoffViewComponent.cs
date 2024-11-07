using FoodOrder.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Repository.Components
{
    public class SaleoffViewComponent:ViewComponent
    {
        private readonly DataContext _dataContext;
        public SaleoffViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var foods = await _dataContext.Foods.Where(c=>c.Status == true).Include(d=>d.Category).Skip(6).Take(6).ToListAsync();


            return View(foods);
        }

    }
}
