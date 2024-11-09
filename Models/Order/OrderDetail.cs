using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrder.Models.Order
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; } 

        

        public string OrderCode { get; set; }  

        public double Sold { get; set; }  

        public int Quantity_Sold { get; set; }  

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]

        public OrderModel Order { get; set; }
        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUserModel AppUser { get; set; }
    }
}
