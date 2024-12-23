﻿using FoodOrder.Data;
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

        public async Task<IActionResult> Index(string Slug = "", string sort_by = "", string startprice = "", string endprice = "", int pg = 1, int pageSize = 8)
        {
            
            CategoryModel category = await _dataContext.Categories
                .Where(c => c.Slug == Slug)
                .FirstOrDefaultAsync();

           
            if (category == null)
            {
                return RedirectToAction("Index");
            }

            // Lọc sản phẩm theo category
            IQueryable<FoodModel> productsByCategory = _dataContext.Foods
                .Where(d => d.CategoryId == category.Id);

            // Đếm tổng số sản phẩm theo category
            var count = await productsByCategory.CountAsync();

            // Nếu có sản phẩm
            if (count > 0)
            {
                // Sắp xếp sản phẩm theo giá trị sort_by
                if (sort_by == "price_increase")
                {
                    productsByCategory = productsByCategory.OrderBy(d => d.Price);
                }
                else if (sort_by == "price_decrease")
                {
                    productsByCategory = productsByCategory.OrderByDescending(d => d.Price);
                }
                else if (sort_by == "price_newest")
                {
                    productsByCategory = productsByCategory.OrderByDescending(d => d.Id);
                }
                else if (sort_by == "price_oldest")
                {
                    productsByCategory = productsByCategory.OrderBy(d => d.Id);
                }

                // Lọc theo giá nếu startprice và endprice được truyền vào
                if (!string.IsNullOrEmpty(startprice) && !string.IsNullOrEmpty(endprice))
                {
                    if (double.TryParse(startprice, out double startPriceValue) && double.TryParse(endprice, out double endPriceValue))
                    {
                        productsByCategory = productsByCategory
                            .Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
                    }
                }

                int totalItems = await productsByCategory.CountAsync();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Gán giá trị cho ViewBag khi có sản phẩm
                ViewBag.PageNumber = pg;
                ViewBag.TotalPages = totalPages;
                ViewBag.Slug = Slug ?? "";  // Gán giá trị mặc định cho Slug nếu null
                ViewBag.SortBy = sort_by ?? "";  // Gán giá trị mặc định cho sort_by nếu null
                ViewBag.StartPrice = startprice ?? "";  // Gán giá trị mặc định cho startprice nếu null
                ViewBag.EndPrice = endprice ?? "";  // Gán giá trị mặc định cho endprice nếu null

                productsByCategory = productsByCategory
                    .Skip((pg - 1) * pageSize)  
                    .Take(pageSize);            

                return View(await productsByCategory.ToListAsync());
            }
            else
            {
               
                ViewBag.PageNumber = null;
                ViewBag.TotalPages = null;
                ViewBag.Slug = null;
                ViewBag.SortBy = null;
                ViewBag.StartPrice = null;
                ViewBag.EndPrice = null;

                
                return View(new List<FoodModel>());
            }
        }







    }
}
