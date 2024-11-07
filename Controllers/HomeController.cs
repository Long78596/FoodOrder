using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Migrations;
using FoodOrder.Models;
using FoodOrder.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FoodOrder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _dataContext;
        private readonly INotyfService _notyfService;

        public HomeController(ILogger<HomeController> logger,DataContext dataContext,INotyfService notyfService)
        {
            _logger = logger;
            _dataContext = dataContext;
            _notyfService = notyfService;
               
        }

        public IActionResult Index()
        {
            var foods = _dataContext.Foods.Include(d => d.Category).Skip(1).Take(8).ToList();
            return View(foods);
        }

        public IActionResult Shop()
        {
            var foods = _dataContext.Foods.Include(d => d.Category).ToList();
            return View(foods);
        }
        public async Task<IActionResult> Search(string searchtern)
        {
            var products = await _dataContext.Foods.Where(p => p.Title.Contains(searchtern) || p.Description.Contains(searchtern)).ToListAsync();
            ViewBag.Keyword = searchtern;
            return View(products);
        }
        public async Task<IActionResult> Detail(int Id)
        {
            if (Id == null) return RedirectToAction("Index");

            var ProductById = _dataContext.Foods
                .Where(c => c.Id == Id).FirstOrDefault();

            if (ProductById == null) return RedirectToAction("Index");

            var relatedProducts = await _dataContext.Foods
                .Where(p => p.CategoryId == ProductById.CategoryId && p.Id != ProductById.Id)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;


            var comments = await _dataContext.Ratings
              .Where(c => c.FoodId == Id)
              .OrderByDescending(c => c.Id)
              .ToListAsync();

            if (comments == null || !comments.Any())
            {
                ViewBag.Comments = new List<RatingModel>(); 
            }
            else
            {
                ViewBag.Comments = comments;
            }


            var viewModel = new FoodDetailViewModel
            {
                Foods = ProductById,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentFood(RatingModel rating)
        {
            if (ModelState.IsValid)
            {
                var ratingEntity = new RatingModel
                {
                    FoodId = rating.FoodId,
                    Name = rating.Name,
                    Comment = rating.Comment,
                    Star = rating.Star,
                };
                _dataContext.Ratings.Add(ratingEntity);
                await _dataContext.SaveChangesAsync();
                _notyfService.Success("Thêm đánh giá thành công");
                return Redirect(Request.Headers["Referer"]);

            }
            else
            {
                return RedirectToAction("Detail", new { id = rating.FoodId });
            }

            return RedirectToAction("Detail");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return PartialView(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

        }
    }
}
