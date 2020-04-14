using MimeKit;

namespace Skyhop.Mail.Abstractions
{
    /// <summary>
    /// This class represent all the fields of a the mail message that can be updated before sending.
    /// </summary>
    public interface IMessageSenderInfo
    {
        /// <summary>
        /// Gets or sets the address in the Sender header.
        /// </summary>
        /// <remarks>
        /// The sender may differ from the addresses in MimeKit.MimeMessage.From if the message
        /// was sent by someone on behalf of someone else.
        /// </remarks>
        public MailboxAddress Sender { get; set; }
        /// <summary>
        /// Gets or sets the address in the Resent-Sender header.
        /// </summary>
        /// <remarks>
        /// The resent sender may differ from the addresses in MimeKit.MimeMessage.ResentFrom
        /// if the message was sent by someone on behalf of someone else.
        /// </remarks>
        public MailboxAddress ResentSender { get; set; }
        /// <summary>
        /// Gets the list of addresses in the From header.
        /// </summary>
        /// <remarks>
        /// The "From" header specifies the author(s) of the message.
        /// If more than one MimeKit.MailboxAddress is added to the list of "From" addresses,
        /// the MimeKit.MimeMessage.Sender should be set to the single MimeKit.MailboxAddress
        /// of the personal actually sending the message.
        /// </remarks>
        public InternetAddressList From { get; }
        /// <summary>
        /// Gets the list of addresses in the Resent-From header.
        /// </summary>
        /// <remarks>
        /// The "Resent-From" header specifies the author(s) of the messagebeing resent.
        /// If more than one MimeKit.MailboxAddress is added to the list of "Resent-From"
        /// addresses, the MimeKit.MimeMessage.ResentSender should be set to the single MimeKit.MailboxAddress
        /// of the personal actually sending the message.
        /// </remarks>
        public InternetAddressList ResentFrom { get; }
        /// <summary>
        /// Gets the list of addresses in the Reply-To header.
        /// </summary>
        /// <remarks>
        /// When the list of addresses in the Reply-To header is not empty, it contains the
        /// address(es) where the author(s) of the message prefer that replies be sent.
        /// When the list of addresses in the Reply-To header is empty, replies should be
        /// sent to the mailbox(es) specified in the From header.
        /// </remarks>
        public InternetAddressList ReplyTo { get; }
        /// <summary>
        /// Gets the list of addresses in the Resent-Reply-To header.
        /// </summary>
        /// <remarks>
        /// When the list of addresses in the Resent-Reply-To header is not empty, it contains
        /// the address(es) where the author(s) of the resent message prefer that replies
        /// be sent.
        /// When the list of addresses in the Resent-Reply-To header is empty, replies should
        /// be sent to the mailbox(es) specified in the Resent-From header.
        /// </remarks>
        public InternetAddressList ResentReplyTo { get; }
        /// <summary>
        /// Gets the list of addresses in the To header.
        /// </summary>
        /// <remarks>
        /// The addresses in the To header are the primary recipients of the message.
        /// </remarks>
        public InternetAddressList To { get; }
        /// <summary>
        /// Gets the list of addresses in the Resent-To header.
        /// </summary>
        /// <remarks>
        /// The addresses in the Resent-To header are the primary recipients of the message.
        /// </remarks>
        public InternetAddressList ResentTo { get; }
        /// <summary>
        /// Gets the list of addresses in the Cc header.
        /// </summary>
        /// <remarks>
        /// The addresses in the Cc header are secondary recipients of the message and are
        /// usually not the individuals being directly addressed in the content of the message.
        /// </remarks>
        public InternetAddressList Cc { get; }
        /// <summary>
        /// Gets the list of addresses in the Resent-Cc header.
        /// </summary>
        /// <remarks>
        /// The addresses in the Resent-Cc header are secondary recipients of the message
        /// and are usually not the individuals being directly addressed in the content of
        /// the message.
        /// </remarks>
        public InternetAddressList ResentCc { get; }
        /// <summary>
        /// Gets the list of addresses in the Bcc header.
        /// </summary>
        /// <remarks>
        /// Recipients in the Blind-Carpbon-Copy list will not be visible to the other recipients
        /// of the message.
        /// </remarks>
        public InternetAddressList Bcc { get; }
        /// <summary>
        /// Gets the list of addresses in the Resent-Bcc header.
        /// </summary>
        /// <remarks>
        /// Recipients in the Resent-Bcc list will not be visible to the other recipients
        /// of the message.
        /// </remarks>
        public InternetAddressList ResentBcc { get; }
    }
}
