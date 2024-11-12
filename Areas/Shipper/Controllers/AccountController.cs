
using FoodOrder.Areas.Shipper.Models;
using FoodOrder.Areas.Shipper.Services;
using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using FoodOrder.Areas.Admin.Repository;

namespace FoodOrder.Areas.Shipper.Controllers
{
    [Area("Shipper")]

    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly INotyfService _notyfService;
        private readonly IEmailSendercs _emailSendercs;
        public AccountController(DataContext dataContext, INotyfService notyfService,IEmailSendercs emailSendercs)
        {
            _dataContext = dataContext;
            _notyfService = notyfService;
            _emailSendercs = emailSendercs;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userExists = await _dataContext.Shippers.FirstOrDefaultAsync(p => p.UserName == taikhoan.UserName.Trim().ToLower());

                    if (userExists != null)
                    {

                        _notyfService.Error("Tên Tài khoản  đã tồn tại vui lòng thử lại!");
                        return PartialView(taikhoan);
                    }
                    var emailExists = await _dataContext.Shippers.FirstOrDefaultAsync(p => p.Email == taikhoan.Email.Trim().ToLower());

                    if (emailExists != null)
                    {

                        _notyfService.Error("Email đã tồn tại vui lòng thử lại!");
                        return PartialView(taikhoan);
                    }
                    var PhoneExists = await _dataContext.Shippers.FirstOrDefaultAsync(p => p.PhoneNumber == taikhoan.PhoneNumber.Trim().ToLower());

                    if (PhoneExists != null)
                    {

                        _notyfService.Error("Số điện thoại đã tồn tại vui lòng thử lại!");
                        return PartialView(taikhoan);
                    }

                    ShipperModel shipper = new ShipperModel
                    {
                        UserName = taikhoan.UserName,
                        PhoneNumber = taikhoan.PhoneNumber.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        Password = (taikhoan.Password.Trim()).ToMD5(),

                    };
                    try
                    {
                        _dataContext.Add(shipper);
                        await _dataContext.SaveChangesAsync();
                        HttpContext.Session.SetString("Id", shipper.Id.ToString());
                        var shipperId = HttpContext.Session.GetString("Id");
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, shipper.UserName),
                            new Claim("Id", shipper.Id.ToString())

                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        _notyfService.Success("Đã đăng ký thành công!");

                        return RedirectToAction("Login", "Account");

                    }
                    catch
                    {
                        return RedirectToAction("Register", "Account");
                    }
                }
                else
                {
                    return PartialView(taikhoan);
                }
            }
            catch
            {
                return PartialView(taikhoan);
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var shipper = await _dataContext.Shippers
                        .FirstOrDefaultAsync(p => p.UserName == login.UserName.Trim().ToLower());

                    if (shipper == null)
                    {
                        _notyfService.Error("Tài khoản không tồn tại, vui lòng thử lại!");
                        return PartialView(login);
                    }

                    string hashedPassword = login.Password.ToMD5();
                    if (shipper.Password != hashedPassword)
                    {
                        _notyfService.Error("Sai thông tin đăng nhập!");
                        return PartialView(login);
                    }

                    HttpContext.Session.SetString("Id", shipper.Id.ToString());

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, shipper.UserName),
                new Claim("Id", shipper.Id.ToString()),
                new Claim(ClaimTypes.Role, "Shipper")
            };

                    var claimsIdentity = new ClaimsIdentity(claims, "ShipperScheme");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync("ShipperScheme", claimsPrincipal);

                    _notyfService.Success("Đã đăng nhập thành công!");
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                _notyfService.Error("Đã xảy ra lỗi, vui lòng thử lại sau!");
            }

            return PartialView(login);
        }
        
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("Id");
            _notyfService.Error("Bạn đã đăng xuất");
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            
            var taikhoanId = HttpContext.Session.GetString("Id");
            if (taikhoanId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(taikhoanId, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _dataContext.Shippers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);

           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfoAccount( ShipperModel model)
        {
            var userById = HttpContext.Session.GetString("Id");
            if (!int.TryParse(userById, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var user = await _dataContext.Shippers.FirstOrDefaultAsync(u =>u.Id==userId);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                user.UserName = model.UserName;
                user.PhoneNumber = model.PhoneNumber;
                user.Password = model.Password.Trim().ToMD5(); 
                user.Email = model.Email;

                _dataContext.Shippers.Update(user);
                await _dataContext.SaveChangesAsync();
                _notyfService.Success("Cập nhật thông tin thành công!");

                return Redirect(Request.Headers["Referer"]);
            }

            return View(model);
        }

        public async Task<IActionResult> NewPass(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                _notyfService.Error("Thiếu email hoặc token.");
                return RedirectToAction("ForgetPass", "Account");
            }
            var checkuser = await _dataContext.Shippers
                .Where(u => u.Email == email && u.Token == token)
                .FirstOrDefaultAsync();
            Console.WriteLine(checkuser);

            if (checkuser != null)
            {
                ViewBag.Email = checkuser.Email;
                ViewBag.Token = token;  
            }
            else
            {
                _notyfService.Error("Email không tồn tại hoặc token không đúng");
                return RedirectToAction("ForgetPass", "Account");
            }
            return View();
        }

        public async Task<IActionResult> ForgetPass(string returnUrl)
        {
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> SendMailForgetPass(ShipperModel user)
        {
            var checkMail = await _dataContext.Shippers.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (checkMail == null)
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
                var message = "Click on link to change password" + " <a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?email=" + checkMail.Email + "&token=" + token + " ' ></a>";

                await _emailSendercs.SendEmailAsync(recevier, subject, message);

            }
            _notyfService.Success("An email has been sent to your registered email address with password reset instruction");
            return RedirectToAction("ForgetPass", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateNewPass(ShipperModel user, string token)
        {
            var checkuser = await _dataContext.Shippers.Where(u => u.Email == user.Email).Where(u => u.Token == user.Token).FirstOrDefaultAsync();
            if (checkuser != null)
            {
                string newtoken = Guid.NewGuid().ToString();

                var passwordHasher = new PasswordHasher<ShipperModel>();
                var passwordHash = passwordHasher.HashPassword(checkuser, user.Password);
                checkuser.Password= passwordHash;

                checkuser.Token = newtoken;

                await _dataContext.SaveChangesAsync();
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
