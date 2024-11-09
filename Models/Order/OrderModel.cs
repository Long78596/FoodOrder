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
        public string Phone { get; set; }
        public string Address { get; set; } // Địa chỉ cụ thể
        public string? OrderNotes { get; set; } // Ghi chú đơn hàng


    }
}
