using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodOrder.Controllers
{
    public class CustomerController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;
        private readonly INotyfService _notyfService;
        private readonly SignInManager<AppUserModel> _signInManager;
        public CustomerController(DataContext context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager, INotyfService notyfService, SignInManager<AppUserModel> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _notyfService = notyfService;
            _signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateInfoAccount(AppUserModel user)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userById = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userById == null)
            {
                return NotFound();
            }else
            {

                var passwordHasher = new PasswordHasher<AppUserModel>();
                var passwordHash = passwordHasher.HashPassword(userById, user.PasswordHash);
                userById.PasswordHash = passwordHash;
                _context.Update(userById);
                await _context.SaveChangesAsync();
                _notyfService.Success("Cập nhật tông tin thành công!");
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");

        }
    }
}
