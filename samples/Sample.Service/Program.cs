using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sample.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMailDispatcher(builder =>
                    {
                        builder.DefaultFromAddress = new MimeKit.MailboxAddress("Email Support", "support@example.tld");

                        builder.MailSender = async message =>
                        {
                            using (var client = new SmtpClient())
                            {
                                await client.ConnectAsync("mail.example.tld", 587, false);
                                await client.AuthenticateAsync("support@example.tld", "**ExamplePassword**");
                                await client.SendAsync(message);
                                await client.DisconnectAsync(true);
                            }
                        };
                    });
                    services.AddHostedService<Worker>();
                });
    }
}
