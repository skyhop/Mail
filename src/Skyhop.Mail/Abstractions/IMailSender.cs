using MimeKit;
using System.Threading.Tasks;

namespace Skyhop.Mail.Abstractions
{
    public interface IMailSender
    {
        Task SendMail(MimeMessage message);
    }
}
