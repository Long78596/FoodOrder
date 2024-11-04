using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly INotyfService _notyfService;
        public CategoryController(DataContext dataContext, INotyfService notyfService)
        {
            _dataContext = dataContext;
            _notyfService = notyfService;
        }
        [HttpGet]
        public IActionResult Index()
        {
           var category = _dataContext.Categories.OrderByDescending(c => c.Id).ToList();
           return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {

           return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
           category.Slug = category.Title.Replace(" ", "-");
           var category_name= await _dataContext.Categories.FirstOrDefaultAsync(p => p.Title == category.Title);
           if (category_name != null)
           {
                _notyfService.Error("Danh mục đã có trong đã tồn tại!");
               return View(category);
           }
           _dataContext.Add(category);
           await _dataContext.SaveChangesAsync();
            _notyfService.Success("Thêm mới danh mục thành công!");
           return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
           var category = await _dataContext.Categories.FindAsync(Id);
           if (category == null)
           {
               return NotFound(); // Return 404 if not found
           }
           return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel categorymodel)
        {
            
                var exists_category = await _dataContext.Categories.FindAsync(categorymodel.Id);
                categorymodel.Slug = categorymodel.Title.Replace(" ", "-");
                var category_name = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Title == categorymodel.Title);

                if (category_name != null && category_name.Id != categorymodel.Id)
                {
                    _notyfService.Error("Danh mục  đã tồn tại!");
                    return View(category_name);
                }
                if (exists_category == null)
                {
                    return NotFound();
                }
                exists_category.Title = categorymodel.Title;
                exists_category.Description = categorymodel.Description;
                exists_category.Status = categorymodel.Status;
                _dataContext.Update(exists_category);
                await _dataContext.SaveChangesAsync();
                _notyfService.Success("Cập nhật danh mục thành công!");
                return RedirectToAction("Index");
            
        }


        public async Task<IActionResult> Delete(int Id)
        {
            var category = await _dataContext.Categories.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }
            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
             _notyfService.Success("Xóa danh mục thành công!");
            return RedirectToAction("Index");
        }
    }
}
