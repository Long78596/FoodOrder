using FoodOrder.Models.Order;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public OrderModel Order { get; set; }

    }
}
