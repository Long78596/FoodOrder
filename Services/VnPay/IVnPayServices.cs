using FoodOrder.Models.VnPay;

namespace FoodOrder.Services.VnPay
{
    public interface IVnPayServices
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
   
}
