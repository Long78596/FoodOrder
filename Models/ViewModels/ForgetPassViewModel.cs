using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Models.ViewModels
{
    public class ForgetPassViewModel
    {
        
        [Required(ErrorMessage = "Yêu cầu nhập email  ")]
        public string Email { get; set; }
    }
}
