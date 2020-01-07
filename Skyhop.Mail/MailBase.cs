using MimeKit;

namespace Skyhop.Mail
{
    public class MailBase
    {
        public MimeMessage MailMessage { get; } = new MimeMessage();
        public BodyBuilder BodyBuilder { get; } = new BodyBuilder();
    }
}
