using FoodOrder.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Data
{
    
    public class DataContext:IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<FoodModel> Foods { get; set; }
        public DbSet<CouponModel> Coupons { get; set; }
        public DbSet<ShippingModel> Shippings  { get; set; }
    }
}
