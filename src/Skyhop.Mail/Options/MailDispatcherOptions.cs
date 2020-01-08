using MimeKit;
using System;
using System.Threading.Tasks;

namespace Skyhop.Mail.Options
{
    public class MailDispatcherOptions
    {
        public MailboxAddress? DefaultFromAddress { get; set; }
        public Func<MimeMessage, Task>? MailSender { get; set; }
    }
}
