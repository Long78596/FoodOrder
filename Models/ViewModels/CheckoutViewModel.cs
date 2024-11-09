namespace FoodOrder.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public double TotalPrice { get; set; }
    }
}
