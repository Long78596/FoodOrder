using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Yêu cầu nhập tên  ")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số điện thoại  ")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có từ 10 đến 11 chữ số.")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập email  ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập địa chỉ ")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập ghi chú ")]
        public string OrderNotes { get; set; }
       
        public List<CartItemModel> CartItems { get; set; }
        public double TotalPrice { get; set; }
        public double ShippingCost { get; set; }

    }

}
