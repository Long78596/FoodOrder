using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Models
{
    public class FoodModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MinLength(4, ErrorMessage = " Yêu cầu nhập tên món ăn")]
        public string Title { get; set; }

        public string Slug { get; set; }
        public string Image { get; set; }

        [Required, MinLength(4, ErrorMessage = " Yêu cầu nhập mô tả món ăn")]
        public string Description { get; set; }

        public bool Status { get; set; }
        public string? servingsize { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập giá món ăn")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá món ăn phải lớn hơn 0")]
        public double DonGia { get; set; }

        

        [Required(ErrorMessage = "Yêu cầu nhập Ngày tạo")]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = " Chọn 1 danh mục")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public CategoryModel Category { get; set; }

        [NotMapped]
        [FileExtensions]
        public IFormFile? ImageUpload { get; set; }
    }
}
