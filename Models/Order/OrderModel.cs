using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrder.Models.Order
{
    public class OrderModel
    {
        [Key]
        public string Id { get; set; }
        public string OrderCode { get; set; }
        public double? ShippingCost { get; set; }
        public string? Coupon { get; set; }
        public string? UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; } 
        public string? OrderNotes { get; set; } 
        public DateTime? Delivery_Date { get; set; }
        public int? Delivery_Status { get; set; }
        public string? PaymentMethod { get; set; }
        public int? ShipperId { get; set; }
        [ForeignKey("ShipperId")]
        public ShipperModel Shipper { get; set; }
        public string? AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUserModel appUserModel { get; set; }



    }
}
