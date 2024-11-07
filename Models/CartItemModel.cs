namespace FoodOrder.Models
{
    public class CartItemModel
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total
        {
            get { return Quantity * Price; }
        }
        public CartItemModel()
        {

        }
        public string Image { get; set; }
        public CartItemModel(FoodModel food)
        {
            FoodId = food.Id;
            FoodName = food.Title;
            Price = food.Price;
            Quantity = 1;
            Image = food.Image;
        }
    }
}
