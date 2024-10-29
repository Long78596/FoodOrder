using FoodOrder.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<FoodModel> Foods { get; set; }
    }
}
