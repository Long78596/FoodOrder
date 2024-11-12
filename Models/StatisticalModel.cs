namespace FoodOrder.Models
{
	public class StatisticalModel
	{
		public int Id { get; set; }
		public  int Quantity_Sold {get;set; }
		public int Sold_Order { get; set; }
		public int Revenue { get; set; }
		public int Profit { get; set; }
		public DateTime DateCreated { get; set; }


	}
}
