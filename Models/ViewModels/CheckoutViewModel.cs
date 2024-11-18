using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Yêu cầu nhập tên  ")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số điện thoại  ")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập email  ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập địa chỉ ")]
        public string Address { get; set; }

        public string OrderNotes { get; set; }
        public List<CartItemModel> CartItems { get; set; }
        public double TotalPrice { get; set; }
    }

}
