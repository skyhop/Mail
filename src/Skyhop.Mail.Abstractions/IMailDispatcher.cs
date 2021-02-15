using MimeKit;
using System;
using System.Threading.Tasks;

namespace Skyhop.Mail.Abstractions
{
    /// <summary>
    /// Interface to a class used to generate and send emails
    /// </summary>
    public interface IMailDispatcher
    {
        /// <summary>
        /// Sends the email
        /// </summary>
        /// <typeparam name="T">The type of the model carrying the payload of the mail.</typeparam>
        /// <param name="data">The message payload</param>
        /// <param name="to">The addresses to which the mail must be sent</param>
        /// <param name="cc">The addresses to which the mail must be cc'ed.</param>
        /// <param name="bcc">The addresses to which the mail must be bcc'ed.</param>
        /// <param name="from">The addresses from which the mail is sent, can be null.</param>
        /// <returns>An awaitable <seealso cref="Task"/> which represents this method call.</returns>
        public Task SendMail<T>(
            T data,
            MailboxAddress[] to,
            MailboxAddress[]? cc = default,
            MailboxAddress[]? bcc = default,
            MailboxAddress? from = default) where T : MailBase;

        /// <summary>
        /// Sends the email
        /// </summary>
        /// <typeparam name="T">The type of the model carrying the payload of the mail.</typeparam>
        /// <param name="data">The message payload</param>
        /// <param name="senderTransformation">An async action that can be used to set the different properties on the <seealso cref="IMessageSenderInfo"/>. The To and From properties must be set.</param>
        /// <returns>An awaitable <seealso cref="Task"/> which represents this method call.</returns>
        public Task SendMail<T>(
            T data,
            Func<IMessageSenderInfo, Task> senderTransformation) where T : MailBase;
    }
}
