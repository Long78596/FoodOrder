using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Models
{
    public class ShipperModel
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Yêu cầu nhập tên ")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số điện thoại")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập mật khẩu")]

        public string Password { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập email")]
        public string Email { get; set; }
        public string? Token { get; set; }
    }
}
