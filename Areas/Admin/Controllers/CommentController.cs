using FoodOrder.Data;
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
        public CommentController(DataContext context)
        {
            _dataContext = context;
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
    }
}
