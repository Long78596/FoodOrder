﻿using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

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
            var foodnew = _dataContext.Foods.Where(x=>x.Status==true).Include(d => d.Category).Skip(8).Take(6).ToList();
            var Topfood = _dataContext.Foods.Where(x=>x.Status==true).Include(d => d.Category).Skip(14).Take(6).ToList();
            ViewBag.foodnew = foodnew;
            ViewBag.Topfood = Topfood;
            return View(foods);
        }

        public async Task<IActionResult> Shop(string sort_by = "", string startprice = "", string endprice = "", int pg = 1)
        {
            int pageSize = 6;  
            var productByCategory = await _dataContext.Foods.Include(d => d.Category).ToListAsync();

           
            if (productByCategory.Any())
            {
                if (sort_by == "price_increase")
                {
                    productByCategory = productByCategory.OrderBy(d => d.Price).ToList();
                }
                else if (sort_by == "price_decrease")
                {
                    productByCategory = productByCategory.OrderByDescending(d => d.Price).ToList();
                }
                else if (sort_by == "price_newest")
                {
                    productByCategory = productByCategory.OrderByDescending(d => d.Id).ToList();
                }
                else if (sort_by == "price_oldest")
                {
                    productByCategory = productByCategory.OrderBy(d => d.Id).ToList();
                }

                // Apply price range filter
                if (!string.IsNullOrEmpty(startprice) && !string.IsNullOrEmpty(endprice))
                {
                    if (double.TryParse(startprice, out double startPriceValue) && double.TryParse(endprice, out double endPriceValue))
                    {
                        productByCategory = productByCategory
                            .Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue)
                            .ToList();
                    }
                }
                else
                {
                    productByCategory = productByCategory.OrderByDescending(p => p.Id).ToList();
                }
            }

            // Pagination
            var totalItems = productByCategory.Count();
            var products = productByCategory.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            ViewBag.CurrentPage = pg;
            ViewBag.TotalPages = totalPages;
            ViewBag.SortBy = sort_by;
            ViewBag.StartPrice = startprice;
            ViewBag.EndPrice = endprice;

            return View(products);
        }




        
        public async Task<IActionResult> Search(string searchtern)
        {
            if (string.IsNullOrWhiteSpace(searchtern))
            {
                _notyfService.Error("Vui lòng nhập từ khóa tìm kiếm! ");
                return Redirect(Request.Headers["Referer"]);
            }
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
            // Danh sách các từ nhạy cảm
            var list = new List<string> { "bậy bạ", "từxấu2", "khôngphùhợp", "con cu", "con cac" }; 

            var wordCount = rating.Comment?.Split(' ').Length ?? 0;
            if (wordCount > 100)
            {

                _notyfService.Error("Bình luận chứa quá 100 từ! ");
                return Redirect(Request.Headers["Referer"]);
            }

            // Kiểm tra từ nhạy cảm
            if (list.Any(word => rating.Comment.Contains(word, StringComparison.OrdinalIgnoreCase)))
            {
                _notyfService.Error("Bình luận chứa từ ngữ ko phù hợp! ");
                return Redirect(Request.Headers["Referer"]);
            }

            
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
