using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MimeKit;
using Skyhop.Mail.Abstractions;

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
                    services.AddSingleton<IMailSender, SmtpMailSender>(); // <== Insert your own implementation
                    services.AddMailDispatcher(builder =>
                    {
                        builder.DefaultFromAddress = new MailboxAddress("Email Support", "support@example.tld");
                    });
                    services.AddHostedService<Worker>();
                });
    }
}
