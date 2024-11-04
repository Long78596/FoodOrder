using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Repository.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower(); // Chuyển đổi phần mở rộng thành chữ thường

                string[] extensions = { "jpg", "png", "jpeg", "webp" };
                bool result = extensions.Any(x => extension.EndsWith(x));
                if (!result)
                {
                    return new ValidationResult("Allowed extensions are jpg, png,webp, or jpeg");
                }
            }
            return ValidationResult.Success;
        }

    }
}
