using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrder.Models.Order
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public double? ShippingCost { get; set; }
        public string? Coupon { get; set; }
        public string? UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Status { get; set; }
        public int FoodId { get; set; }
        [ForeignKey("FoodId")]

        public FoodModel Food { get; set; }
    }
}
