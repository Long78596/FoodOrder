using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FoodController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly INotyfService _notyfService;
        private readonly IWebHostEnvironment _webHostEnviorment;
        public FoodController(DataContext dataContext, IWebHostEnvironment webHostEnviorment, INotyfService notyfService)
        {
            _dataContext = dataContext;
            _webHostEnviorment = webHostEnviorment;
            _notyfService = notyfService;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _dataContext.Foods.OrderByDescending(p => p.Id).Include(c => c.Category).ToListAsync());
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Category = new SelectList(_dataContext.Categories, "Id", "Title");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodModel foods)
        {
            ViewBag.Category = new SelectList(_dataContext.Categories, "Id", "Title", foods.CategoryId);

            foods.Slug = foods.Title.Replace(" ", "-");
            var food_name = await _dataContext.Foods.FirstOrDefaultAsync(p => p.Title == foods.Title);
            if (food_name != null)
            {
                _notyfService.Error("Món ăn đã có trong database");
                return View();
            }

            if (foods.ImageUpload != null)
            {
                string uploadDir = Path.Combine(_webHostEnviorment.WebRootPath, "image/food");

                string imagename = Guid.NewGuid().ToString() + "_" + foods.ImageUpload.FileName;
                string filePath = Path.Combine(uploadDir, imagename);

                FileStream fs = new FileStream(filePath, FileMode.Create);
                await foods.ImageUpload.CopyToAsync(fs);
                fs.Close();
                foods.Image = imagename;

            }
            foods.Created = DateTime.Now;


            _dataContext.Add(foods);
            await _dataContext.SaveChangesAsync();
            _notyfService.Success("Thêm món ăn thành công!");
            return RedirectToAction("Index");


        }
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            FoodModel monAn = await _dataContext.Foods.FindAsync(Id);
            ViewBag.Category = new SelectList(_dataContext.Categories, "Id", "Title");
            return View(monAn);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FoodModel monan)
        {
            ViewBag.Category = new SelectList(_dataContext.Categories, "Id", "Title");


            if (monan.Id == null)
            {
                _notyfService.Error("Mã món ăn không hợp lệ!");
                return View(monan);
            }

            var exists_food = await _dataContext.Foods.FindAsync(monan.Id);


            if (exists_food == null)
            {
                _notyfService.Error("Món ăn không tồn tại!");
                return View(monan);
            }

            monan.Slug = monan.Title.Replace(" ", "-");
            var tenmonan = await _dataContext.Foods.FirstOrDefaultAsync(p => p.Title == monan.Title );

            if (tenmonan != null)
            {
                _notyfService.Error("Món ăn đã tồn tại!");
                return View(monan);
            }

            if (monan.ImageUpload != null)
            {
                string uploadDir = Path.Combine(_webHostEnviorment.WebRootPath, "image/food");
                string imageName = Guid.NewGuid().ToString() + "_" + monan.ImageUpload.FileName;
                string filePath = Path.Combine(uploadDir, imageName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await monan.ImageUpload.CopyToAsync(fs);
                }
                exists_food.Image = imageName;
            }

            exists_food.Title = monan.Title;
            exists_food.Description = monan.Description;
            //exists_food.DiaChiQuan = monan.DiaChiQuan;
            exists_food.Quantity = monan.Quantity;
            exists_food.Price = monan.Price;
            //exists_food.Video = monan.Video;
            exists_food.CategoryId = monan.CategoryId;
            exists_food.Created = monan.Created;
            exists_food.Status = monan.Status;

            _dataContext.Update(exists_food);
            await _dataContext.SaveChangesAsync();
            _notyfService.Success("Cập nhật món ăn thành công!");
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Delete(int Id)
        {
            FoodModel monan = await _dataContext.Foods.FindAsync(Id);
            if (!string.Equals(monan.Image, "noname.jpg"))
            {
                string uploadDir = Path.Combine(_webHostEnviorment.WebRootPath, "image/food");
                string filePath = Path.Combine(uploadDir, monan.Image);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            _dataContext.Foods.Remove(monan);
            await _dataContext.SaveChangesAsync();
            _notyfService.Success("Xóa món ăn thành công!");
            return RedirectToAction("Index");
        }
       
        [HttpGet]
        public IActionResult Loc(int CateId = 0)
        {
            var url = $"/Admin/MonAn?CateID={CateId}";
            if (CateId == 0)
            {
                url = $"/Admin/MonAn";
            }
            return Json(new { status = "success", redirectUrl = url });
        }
    }
}

