using FoodOrder.Models.VnPay;
using FoodOrder.Services.VnPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IVnPayServices _vnPayService;
        public PaymentController(IVnPayServices vnPayService)
        {

            _vnPayService = vnPayService;
        }
        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
          
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
        


    }
}
