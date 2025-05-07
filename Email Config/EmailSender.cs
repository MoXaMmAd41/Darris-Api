using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Darris_Api.Email_Config
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }


      public async Task SendEmailAsync(string ToEmail, string Subject, string Message)
            {
            var Mail = new MailMessage()
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = Subject,
                Body = Message,
                IsBodyHtml = true
            };
            Mail.To.Add(ToEmail);

            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true
            };
            await smtp.SendMailAsync(Mail);


        }
        

        
              
    }
}
