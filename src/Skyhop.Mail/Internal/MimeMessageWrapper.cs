using MimeKit;
using Skyhop.Mail.Abstractions;
using System;
using System.Collections.Generic;

namespace Skyhop.Mail.Internal
{
    internal class MimeMessageWrapper : IMessageContent, IMessageSenderInfo
    {
        internal MimeMessageWrapper(MimeMessage mimeMessage)
        {
            WrappedMessage = mimeMessage;
        }

        internal MimeMessage WrappedMessage { get; }

        /// <inheritdoc />
        public MessageImportance Importance { get => WrappedMessage.Importance; set => WrappedMessage.Importance = value; }
        /// <inheritdoc />
        public MessagePriority Priority { get => WrappedMessage.Priority; set => WrappedMessage.Priority = value; }
        /// <inheritdoc />
        public XMessagePriority XPriority { get => WrappedMessage.XPriority; set => WrappedMessage.XPriority = value; }
        /// <inheritdoc />
        public DateTimeOffset Date { get => WrappedMessage.Date; set => WrappedMessage.Date = value; }
        /// <inheritdoc />
        public DateTimeOffset ResentDate { get => WrappedMessage.ResentDate; set => WrappedMessage.ResentDate = value; }

        /// <inheritdoc />
        public MessageIdList References => WrappedMessage.References;

        /// <inheritdoc />
        public string InReplyTo { get => WrappedMessage.InReplyTo; set => WrappedMessage.InReplyTo = value; }
        /// <inheritdoc />
        public string MessageId { get => WrappedMessage.MessageId; set => WrappedMessage.MessageId = value; }
        /// <inheritdoc />
        public string ResentMessageId { get => WrappedMessage.ResentMessageId; set => WrappedMessage.ResentMessageId = value; }
        /// <inheritdoc />
        public Version MimeVersion { get => WrappedMessage.MimeVersion; set => WrappedMessage.MimeVersion = value; }
        /// <inheritdoc />
        public MimeEntity Body { get => WrappedMessage.Body; set => WrappedMessage.Body = value; }

        /// <inheritdoc />
        public string? TextBody => WrappedMessage.TextBody;

        /// <inheritdoc />
        public string? HtmlBody => WrappedMessage.HtmlBody;

        /// <inheritdoc />
        public HeaderList Headers => WrappedMessage.Headers;

        /// <inheritdoc />
        public IEnumerable<MimeEntity> BodyParts => WrappedMessage.BodyParts;

        /// <inheritdoc />
        public IEnumerable<MimeEntity> Attachments => WrappedMessage.Attachments;

        /// <inheritdoc />
        public MailboxAddress Sender { get => WrappedMessage.Sender; set => WrappedMessage.Sender = value; }
        /// <inheritdoc />
        public MailboxAddress ResentSender { get => WrappedMessage.ResentSender; set => WrappedMessage.ResentSender = value; }

        /// <inheritdoc />
        public InternetAddressList From => WrappedMessage.From;

        /// <inheritdoc />
        public InternetAddressList ResentFrom => WrappedMessage.ResentFrom;

        /// <inheritdoc />
        public InternetAddressList ReplyTo => WrappedMessage.ReplyTo;

        /// <inheritdoc />
        public InternetAddressList ResentReplyTo => WrappedMessage.ResentReplyTo;

        /// <inheritdoc />
        public InternetAddressList To => WrappedMessage.To;

        /// <inheritdoc />
        public InternetAddressList ResentTo => WrappedMessage.ResentTo;

        /// <inheritdoc />
        public InternetAddressList Cc => WrappedMessage.Cc;

        /// <inheritdoc />
        public InternetAddressList ResentCc => WrappedMessage.ResentCc;

        /// <inheritdoc />
        public InternetAddressList Bcc => WrappedMessage.Bcc;

        /// <inheritdoc />
        public InternetAddressList ResentBcc => WrappedMessage.ResentBcc;
    }
}
