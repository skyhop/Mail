using MimeKit;
using System.Threading.Tasks;

namespace Skyhop.Mail.Abstractions
{
    /// <summary>
    /// Interface representing a mailer
    /// </summary>
    public interface IMailSender
    {
        /// <summary>
        /// Sends the provided message
        /// </summary>
        /// <param name="message">The message that needs to be sent.</param>
        /// <returns>An awaitable <seealso cref="Task"/> which represents this method call.</returns>
        Task SendMail(MimeMessage message);
    }
}
