using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Models;
using FoodOrder.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<AppUserModel> _userManager;
        private readonly SignInManager<AppUserModel> _signInManager;
        private readonly INotyfService _notyfService;
        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager,INotyfService notyfService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyfService = notyfService;
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                AppUserModel newUser = new AppUserModel { UserName = user.UserName, Email = user.Email };
                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
                if (result.Succeeded)
                {
                    //_notyfService.Success()

                    return Redirect("/account/login");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            return View(new LoginViewModel{ ReturnUrl = returnUrl });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
          
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(loginVM.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Mật khẩu hoặc password sai ");
            return Redirect(loginVM.ReturnUrl ?? "/");


        }

    }
}
