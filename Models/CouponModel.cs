using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Models
{
    public class CouponModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = " Nhập phiếu khuyến mãi ")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool Status { get; set; }

        public int Quantity { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DataExpired { get; set; }
    }
}
