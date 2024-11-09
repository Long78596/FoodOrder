using FoodOrder.Areas.Admin.Repository;
using FoodOrder.Data;
using FoodOrder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CommentController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly SensitiveWordFilter _sensitiveWordFilter;
        public CommentController(DataContext context, SensitiveWordFilter sensitiveWordFilter)
        {
            _dataContext = context;
            _sensitiveWordFilter = sensitiveWordFilter;
        }

        public async Task<IActionResult> Index()
        {
            var comments = await _dataContext.Ratings.ToListAsync();
            return View(comments);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _dataContext.Ratings.FindAsync(id);
            if (comment != null)
            {
                _dataContext.Ratings.Remove(comment);
                await _dataContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        //[HttpPost]
        //public async Task<IActionResult> AddComment(RatingModel rating)
        //{
        //    if (_sensitiveWordFilter.ContainsSensitiveWords(rating.Comment ?? ""))
        //    {
        //        ModelState.AddModelError("Comment", "Bình luận chứa từ ngữ nhạy cảm. Vui lòng sửa lại.");
        //        return View(rating);
        //    }
        //    if (_sensitiveWordFilter.ExceedsWordLimit(rating.Comment ?? ""))
        //    {
        //        ModelState.AddModelError("Comment", "Bình luận không được vượt quá 100 từ.");
        //        return View(rating);
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        _dataContext.Ratings.Add(rating);
        //        await _dataContext.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    return View(rating);
        //}
    }
}
