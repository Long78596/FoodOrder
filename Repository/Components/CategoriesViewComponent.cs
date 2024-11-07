using FoodOrder.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Repository.Components
{
    public class CategoriesViewComponent:ViewComponent
    {
        private readonly DataContext _dataContext;
        public CategoriesViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var danhMucs = await _dataContext.Categories.ToListAsync();


            return View(danhMucs);
        }
    
    }
}
