using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {

        private readonly DataContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly INotyfService _notyfService;
        public RoleController(DataContext context, RoleManager<IdentityRole> roleManager,INotyfService notyfService)
        {
            _context = context;
            _roleManager = roleManager;
            _notyfService = notyfService;
        }
       
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.OrderByDescending(p => p.Id).ToListAsync());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole model)
        {

            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
                _notyfService.Success("Thêm thành công");
            }
            return Redirect("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            try
            {
                await _roleManager.DeleteAsync(role);
                _notyfService.Success("Xóa thành công");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the role.");
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(Id);
            return View(role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string Id, IdentityRole model)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(Id);
                if (role == null)
                {
                    return NotFound();
                }

                role.Name = model.Name;
                try
                {
                    await _roleManager.UpdateAsync(role);
                    _notyfService.Success("Cập nhật  thành công");

                    return RedirectToAction("Index");


                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occured while updating the role");
                }

            }
            return View(model ?? new IdentityRole { Id = Id });

        }
    }
}
