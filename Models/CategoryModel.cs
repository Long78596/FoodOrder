using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id{ get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên danh mục ")]
        public string Title { get; set; }
        public string Slug { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập nội dung danh mục")]
        public string? Description { get; set; }
        public int Status{ get; set; }
    }
}
