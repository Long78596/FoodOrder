namespace FoodOrder.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string OrderNotes { get; set; }
        public List<CartItemModel> CartItems { get; set; }
        public double TotalPrice { get; set; }
    }

}
