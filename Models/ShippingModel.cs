using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Models
{
    public class ShippingModel
    {
        [Key]  
        
        public int Id { get; set; }
        public double Price { get; set; }
        public string Ward { get; set; }
        public string Dictric { get; set; }
        public string City { get; set; }
    }
}
