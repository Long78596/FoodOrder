using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Models.ViewModels
{
    public class FoodDetailViewModel
    {
           public FoodModel Foods { get; set; }
            [Required(ErrorMessage = "Yêu cầu nhập bình luận sản phẩm")]

            public string Comment { get; set; }

            [Required(ErrorMessage = "Yêu cầu nhập tên")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Yêu cầu nhập email")]
            public string Email { get; set; }
        
    }
}
