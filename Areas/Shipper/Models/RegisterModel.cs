using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Areas.Shipper.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Vui lòng nhập username")]
        public string UserName { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập password")]
        public string PhoneNumber{ get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
