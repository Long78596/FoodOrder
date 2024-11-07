using FoodOrder.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Repository.Components
{
    public class FoodHomeViewComponent:ViewComponent
    {

        private readonly DataContext _dataContext;
        public FoodHomeViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var foods = await _dataContext.Foods.Take(1).FirstOrDefaultAsync();


            return View(foods);
        }

    }
}
