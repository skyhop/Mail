using Skyhop.Mail;
using System;

namespace SampleService.Models
{
    public class ServiceActionModel : MailBase
    {
        public DateTime Timestamp { get; set; }
        public string ActionName { get; set; }
    }
}
