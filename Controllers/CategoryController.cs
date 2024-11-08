using FoodOrder.Data;
using FoodOrder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Index(string Slug = "", string sort_by= "",string startprice= "",string endprice= "")
        {
            CategoryModel category = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();
            
            
            if (category == null) return RedirectToAction("Index");
            IQueryable<FoodModel> productByCategory = _dataContext.Foods.Where(d => d.CategoryId == category.Id);

            var count = await productByCategory.CountAsync();
           if(count > 0)
            {
                if(sort_by== "price_increase")
                {
                    productByCategory = productByCategory.OrderBy(d => d.Price);
                }else if (sort_by== "price_decrease")
                {
                    productByCategory = productByCategory.OrderByDescending(d => d.Price);
                }
                else if (sort_by == "price_newest")
                {
                    productByCategory = productByCategory.OrderByDescending(d => d.Id);
                }
                else if (sort_by == "price_oldest")
                {
                    productByCategory = productByCategory.OrderBy(d => d.Id);
                }
                //lọc giá

                else if (startprice != "" && endprice != "")
                {
                    double startPriceValue;
                    double endPriceValue;
                    if(double.TryParse(startprice, out startPriceValue) && double.TryParse(endprice, out endPriceValue))
                    {
                        productByCategory = productByCategory.Where(p => p.Price >= startPriceValue && p.Price<= endPriceValue);

                    }
                    else
                    {
                        productByCategory = productByCategory.OrderByDescending(p => p.Id);
                    }
                }
                else
                {
                    productByCategory = productByCategory.OrderByDescending(p => p.Id);
                }
            }
            
            
            return View(await productByCategory.ToListAsync());
        }
    }
}
