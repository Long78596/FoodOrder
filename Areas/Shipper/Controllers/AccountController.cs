using AspNetCoreHero.ToastNotification.Abstractions;
using FoodOrder.Areas.Shipper.Models;
using FoodOrder.Areas.Shipper.Services;
using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodOrder.Areas.Shipper.Controllers
{
    [Area("Shipper")]

    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly INotyfService _notyfService;
        public AccountController(DataContext dataContext, INotyfService notyfService)
        {
            _dataContext = dataContext;
            _notyfService = notyfService;
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

    }

}
