using MailKit.Net.Smtp;
using MimeKit;
using Skyhop.Mail.Abstractions;
using System.Threading.Tasks;

namespace Sample.Service
{
    public class SmtpMailSender : IMailSender
    {
        public async Task SendMail(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("mail.example.tld", 587, false);
                await client.AuthenticateAsync("support@example.tld", "**ExamplePassword**");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
