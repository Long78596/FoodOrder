using System.Text.RegularExpressions;

namespace FoodOrder.Repository.Validation
{
    public class EmailValidatorcs
    {

        public bool IsValidEmail(string email)
        {
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase);
        }
    }
}
