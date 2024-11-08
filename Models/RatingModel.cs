using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }

        public int FoodId { get; set; }
        [Required(ErrorMessage = "yêu cầu nhập nội dung sản phẩm")]
        public string? Comment { get; set; }
        [Required(ErrorMessage = "yêu cầu nhập tên ")]
        public string? Name { get; set; }
        public string? Star { get; set; }

        //[ForeignKey("ProductId")]
        //public FoodModel Food{ get; set; }
    }
}
