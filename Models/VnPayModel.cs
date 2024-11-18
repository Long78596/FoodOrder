using FoodOrder.Models.Order;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrder.Models
{
    
        public class VnPayModel
        {
            [Key]
            public int Id { get; set; }

            public string OrderId { get; set; }  

         

            public string OrderDescription { get; set; }
            public string TransactionId { get; set; }
            public string PaymentMethod { get; set; }
            public string PaymentId { get; set; }
        }

    
}
