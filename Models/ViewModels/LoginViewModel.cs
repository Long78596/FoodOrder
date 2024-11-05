using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Models.ViewModels
{
    public class LoginViewModel
    {
     
        [Required(ErrorMessage = "Vui lòng nhập username")]
        public string UserName { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập password")]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
