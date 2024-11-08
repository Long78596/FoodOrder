using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using FoodOrder.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;
        private readonly INotyfService _notyfService;
        private readonly SignInManager<AppUserModel> _signInManager;
        public UserController(DataContext context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager,INotyfService notyfService,SignInManager<AppUserModel> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _notyfService = notyfService;
            _signInManager = signInManager;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var userWithRoles = await (from u in _context.Users
                                       join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                                       from ur in userRoles.DefaultIfEmpty() 
                                       join r in _context.Roles on ur.RoleId equals r.Id into roles
                                       from r in roles.DefaultIfEmpty() 
                                       select new
                                       {
                                           User = u,
                                           RoleName = r != null ? r.Name : "No Role" 
                                       }).ToListAsync();

            return View(userWithRoles);

        }
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (createUserResult.Succeeded)
                {
                    var createdUser = await _userManager.FindByEmailAsync(user.Email);
                    var userId = createdUser.Id;
                    var role = _roleManager.FindByIdAsync(user.RoleId);
                    var addToRoleResult = await _userManager.AddToRoleAsync(createdUser, role.Result.Name);
                    if (!addToRoleResult.Succeeded)
                    {
                        AddIdentityErrors(createUserResult);
                    }
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(createUserResult);
                    return View(user);
                }
            }
            else
            {
                TempData["error"] = " có 1 vài model đang bị lỗi ";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }

                }
                string errorMesage = string.Join("\n", errors);
                return BadRequest(errorMesage);
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        private void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();


            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "Đã xóa thành công";
            return RedirectToAction("Index");

        }
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();

            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Update")]
        public async Task<IActionResult> Update(string Id, AppUserModel model)
        {
            var existing = await _userManager.FindByIdAsync(Id);
            if (existing == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existing.UserName = model.UserName;
                existing.Email = model.Email;
                existing.PhoneNumber = model.PhoneNumber;

                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role == null)
                {
                    ModelState.AddModelError("", "Role không tồn tại.");
                    return View(existing);
                }

                var currentRoles = await _userManager.GetRolesAsync(existing);
                await _userManager.RemoveFromRolesAsync(existing, currentRoles);

                var addRoleResult = await _userManager.AddToRoleAsync(existing, role.Name);
                if (!addRoleResult.Succeeded)
                {
                    AddIdentityErrors(addRoleResult);
                    return View(existing);
                }

                var updateUserResult = await _userManager.UpdateAsync(existing);
                if (updateUserResult.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(updateUserResult);
                    return View(existing);
                }
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(x => x.ErrorMessage)).ToList();
            string errorMessage = string.Join("\n", errors);
            return View(existing);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            await _signInManager.SignOutAsync();
            _notyfService.Error("Đăng xuất thành công");
            return RedirectToAction("Login", "Account");
        }

    }
}
