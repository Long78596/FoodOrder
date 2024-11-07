using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Models;
using FoodOrder.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;
using System.Security.Claims;
using FoodOrder.Areas.Admin.Repository;
using Microsoft.EntityFrameworkCore;
using FoodOrder.Data;

namespace FoodOrder.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<AppUserModel> _userManager;
        private readonly SignInManager<AppUserModel> _signInManager;
        private readonly INotyfService _notyfService;
        private readonly IEmailSendercs _emailSendercs;
        private readonly DataContext _dataContext;
        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager,INotyfService notyfService,IEmailSendercs emailSendercs,DataContext dataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyfService = notyfService;
            _emailSendercs = emailSendercs;
            _dataContext = dataContext;

        }
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView();
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
                    _notyfService.Success("Đăng ký thành công");
                    

                    return Redirect("/account/login");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return PartialView(user);
        }
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            _notyfService.Error("Yêu cầu phải đăng nhập");
            return PartialView(new LoginViewModel{ ReturnUrl = returnUrl });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra nếu username tồn tại
                    var user = await _userManager.FindByNameAsync(loginVM.UserName);
                    if (user == null)
                    {
                        _notyfService.Error("tài khoản không tồn tại");
                        return PartialView(loginVM);
                    }

                    var result = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        _notyfService.Success("Đăng nhập thành công");
                        return Redirect(loginVM.ReturnUrl ?? "/");
                    }
                    _notyfService.Error("username hoặc password bị sai");
                }
                catch (SqlNullValueException ex)
                {
                    Console.WriteLine("Lỗi: " + ex.Message);
                    _notyfService.Error("Đã xảy ra lỗi trong quá trình đăng nhập");
                }
            }
            return PartialView(loginVM);
        }

        public async Task<IActionResult> Logout(string returnUrl= "/")
        {
            await HttpContext.SignOutAsync();
            await _signInManager.SignOutAsync();
            return RedirectToAction(returnUrl);
        }
        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri=Url.Action("GoogleResponse")

                });
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value

            });
            //_notyfService.Success("Đăng nhập thành công");
            //return RedirectToAction("Index", "Home");
            //return Json(claims);
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string UserName = email.Split("")[0];
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                var passwordHasher = new PasswordHasher<AppUserModel>();
                var hashedPassword = passwordHasher.HashPassword(null, "123456qa");
                var newUser = new AppUserModel { UserName = UserName, Email = email };
                newUser.PasswordHash = hashedPassword;
                var createUserResult = await _userManager.CreateAsync(newUser);
                if (!createUserResult.Succeeded)
                {
                    _notyfService.Error("Đăng nhập thất bại");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    _notyfService.Success("Đăng ký thành công");
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                _notyfService.Success("Đăng nhập thành công");
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> NewPass(AppUserModel user, string token)
        {
            var checkuser = await _userManager.Users.Where(u => u.Email == user.Email).Where(u => u.Token == user.Token).FirstOrDefaultAsync();
            if(checkuser != null)
            {
                ViewBag.Email = checkuser.Email;
                ViewBag.Token = token;
            }
            else
            {
                _notyfService.Error("Email ko tồn tại hoặc token ko tồn tại ");
                return RedirectToAction("ForgetPass", "Account");
            }
            return PartialView();
        }
        public async Task<IActionResult> ForgetPass(string returnUrl)
        {
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> SendMailForgetPass(AppUserModel user)
        {
            var checkMail = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if(checkMail == null)
            {
                _notyfService.Error("Email ko tồn tại");
                return RedirectToAction("ForgetPass", "Account");
            }
            else
            {
                string token = Guid.NewGuid().ToString();

                checkMail.Token = token;
                _dataContext.Update(checkMail);
                await _dataContext.SaveChangesAsync();
                var recevier = checkMail.Email;
                var subject = "Change password for user " + checkMail.Email;
                var message = "Click on link to change password" + " <a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?email="+ checkMail.Email + "&token=" +token + " ' ></a>";

                await _emailSendercs.SendEmailAsync(recevier, subject, message);

            }
            _notyfService.Success("An email has been sent to your registered email address with password reset instruction");
            return RedirectToAction("ForgetPass", "Account");
        }
        [HttpPost]   
        public async Task<IActionResult> UpdateNewPass(AppUserModel user, string token)
        {
            var checkuser = await _userManager.Users.Where(u => u.Email == user.Email).Where(u => u.Token == user.Token).FirstOrDefaultAsync();
            if(checkuser != null)
            {
                string newtoken = Guid.NewGuid().ToString();

                var passwordHasher = new PasswordHasher<AppUserModel>();
                var passwordHash = passwordHasher.HashPassword(checkuser, user.PasswordHash);
                checkuser.PasswordHash = passwordHash;

                checkuser.Token = newtoken;

                await _userManager.UpdateAsync(checkuser);
                _notyfService.Success("Cập nhật mật khẩu thành công");
                return RedirectToAction("Login", "Account");
            }
            else
            {
                _notyfService.Error("Email ko tồn tại hoặc token ko tồn tại ");
                return RedirectToAction("ForgetPass", "Account");
            }
        }


    }
}
