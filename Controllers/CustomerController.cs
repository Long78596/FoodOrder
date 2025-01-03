using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.Repository.Validation;
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
        private readonly EmailValidatorcs _emailValidatorcs;
        public CustomerController(DataContext context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager, INotyfService notyfService, SignInManager<AppUserModel> signInManager,EmailValidatorcs emailValidatorcs)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _notyfService = notyfService;
            _signInManager = signInManager;
            _emailValidatorcs = emailValidatorcs;
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

            if (!_emailValidatorcs.IsValidEmail(user.Email))
            {
                _notyfService.Error("Email không đúng định dạng");
                return Redirect(Request.Headers["Referer"]);
            }
            if (user.PasswordHash.Length < 6 || !user.PasswordHash.Any(char.IsLetter) || !user.PasswordHash.Any(char.IsDigit))
            {
                _notyfService.Error("Mật khẩu phải có tối thiểu 6 ký tự và có cả ký tự số và chữ!");
                return Redirect(Request.Headers["Referer"]);
            }
            

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
