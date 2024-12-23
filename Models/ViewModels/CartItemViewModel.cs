﻿namespace FoodOrder.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        public double GrandTotal { get; set; }
        public int Quantity { get; set; }
        public string CouponCode { get; set; }
        public double ShippingCost { get; set; }
    }
}
