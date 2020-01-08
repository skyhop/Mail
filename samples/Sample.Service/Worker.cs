using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using SampleService.Models;
using Skyhop.Mail;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MailDispatcher _mailDispatcher;

        public Worker(
            ILogger<Worker> logger,
            MailDispatcher mailDispatcher)
        {
            _logger = logger;
            _mailDispatcher = mailDispatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _mailDispatcher.SendMail(
                data: new ServiceActionModel
                {
                    ActionName = "Starting",
                    Timestamp = DateTime.UtcNow
                },
                to: new[] { new MailboxAddress("John Doe", "john.doe@example.tld") });

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
