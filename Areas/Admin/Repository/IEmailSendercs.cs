namespace FoodOrder.Areas.Admin.Repository
{
    public interface IEmailSendercs
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
