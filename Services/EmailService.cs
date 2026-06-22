using MailKit.Net.Smtp;
using MimeKit;

namespace SmartWaste.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string messageBody)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Eco Vision Team", _configuration["EmailSettings:From"]));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = messageBody };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // للتجارب بـ Gmail بنستخدم smtp.gmail.com وبورت 587
                await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"],
                                          int.Parse(_configuration["EmailSettings:Port"]),
                                          MailKit.Security.SecureSocketOptions.StartTls);

                // بتعمل App Password من حساب جوجل بتاعك مش الباسورد العادي
                await client.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);

                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
