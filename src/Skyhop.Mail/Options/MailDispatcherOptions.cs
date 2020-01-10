using MimeKit;

namespace Skyhop.Mail.Options
{
    /// <summary>
    /// Options and settings used in the <see cref="MailDispatcher"/>
    /// </summary>
    public class MailDispatcherOptions
    {
        /// <summary>
        /// The default from address used if no other from was specified in the SendMail call
        /// </summary>
        public MailboxAddress? DefaultFromAddress { get; set; }
    }
}
