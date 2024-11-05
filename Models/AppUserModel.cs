using Microsoft.AspNetCore.Identity;

namespace FoodOrder.Models
{
    public class AppUserModel:IdentityUser
    {
        public string? RoleId { get; set; }
    }
}
