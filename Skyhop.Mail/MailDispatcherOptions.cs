using MimeKit;
using System;

namespace Skyhop.Mail
{
    public class MailDispatcherOptions
    {
        public MailboxAddress DefaultFromAddress { get; set; }
        public Action<MimeMessage> MailSender { get; set; }
    }
}
