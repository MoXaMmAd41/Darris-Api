namespace Darris_Api
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string ToEmail,string Subject,string Message);

    }
}
