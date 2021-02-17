using MimeKit;
using System;
using System.Collections.Generic;

namespace Skyhop.Mail.Abstractions
{
    /// <summary>
    /// This class represents the content of the mail message.
    /// </summary>
    public interface IMessageContent
    {
        /// <summary>
        /// Get or set the value of the Importance header.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Value is not a valid MimeKit.MessageImportance.
        /// </exception>
        /// <remarks>
        /// Gets or sets the value of the Importance header.
        /// </remarks>
        public MessageImportance Importance { get; set; }
        /// <summary>
        /// Get or set the value of the Priority header.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// value is not a valid MimeKit.MessagePriority.
        /// </exception>
        /// <remarks>
        /// Gets or sets the value of the Priority header.
        /// </remarks>
        public MessagePriority Priority { get; set; }
        /// <summary>
        /// Get or set the value of the X-Priority header.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// value is not a valid MimeKit.MessagePriority.
        /// </exception>
        /// <remarks>
        /// Gets or sets the value of the X-Priority header.
        /// </remarks>
        public XMessagePriority XPriority { get; set; }
        /// <summary>
        /// Gets or sets the date of the message.
        /// </summary>
        /// <remarks>
        /// If the date is not explicitly set before the message is written to a stream,
        /// the date will default to the exact moment when it is written to said stream.
        /// </remarks>
        public DateTimeOffset Date { get; set; }
        /// <summary>
        /// Gets or sets the Resent-Date of the message.
        /// </summary>
        /// <remarks>
        /// Gets or sets the Resent-Date of the message.
        /// </remarks>
        public DateTimeOffset ResentDate { get; set; }
        /// <summary>
        /// Gets or sets the list of references to other messages.
        /// </summary>
        /// <remarks>
        /// The References header contains a chain of Message-Ids back to the original message
        /// that started the thread.
        /// </remarks>
        public MessageIdList References { get; }
        /// <summary>
        /// Gets or sets the Message-Id that this message is in reply to.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// value is improperly formatted.
        /// </exception>
        /// <remarks>
        /// If the message is a reply to another message, it will typically use the In-Reply-To
        /// header to specify the Message-Id of the original message being replied to.
        /// </remarks>
        public string InReplyTo { get; set; }
        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// value is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// value is improperly formatted.
        /// </exception>
        /// <remarks>
        /// The Message-Id is meant to be a globally unique identifier for a message.
        /// MimeKit.Utils.MimeUtils.GenerateMessageId can be used to generate this value.
        /// </remarks>
        public string MessageId { get; set; }
        /// <summary>
        /// Gets or sets the Resent-Message-Id header.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// value is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// value is improperly formatted.
        /// </exception>
        /// <remarks>
        /// The Resent-Message-Id is meant to be a globally unique identifier for a message.
        /// MimeKit.Utils.MimeUtils.GenerateMessageId can be used to generate this value.
        /// </remarks>
        public string ResentMessageId { get; set; }
        /// <summary>
        /// Gets or sets the MIME-Version.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// value is null.
        /// </exception>
        /// <remarks>
        /// The MIME-Version header specifies the version of the MIME specification that
        /// the message was created for.
        /// </remarks>
        public Version MimeVersion { get; set; }
        /// <summary>
        /// Gets or sets the body of the message.
        /// </summary>
        /// <remarks>
        /// The body of the message can either be plain text or it can be a tree of MIME
        /// entities such as a text/plain MIME part and a collection of file attachments.
        /// For a convenient way of constructing message bodies, see the MimeKit.BodyBuilder
        /// class.
        /// </remarks>
        public MimeEntity Body { get; set; }
        /// <summary>
        /// Gets the text body of the message if it exists.
        /// </summary>
        /// <remarks>
        /// Gets the text content of the first text/plain body part that is found (in depth-first
        /// search order) which is not an attachment.
        /// </remarks>
        public string? TextBody { get; }
        /// <summary>
        /// Gets the html body of the message if it exists.
        /// </summary>
        /// <remarks>
        /// Gets the HTML-formatted body of the message if it exists.
        /// </remarks>
        public string? HtmlBody { get; }
        /// <summary>
        /// Gets the list of headers.
        /// </summary>
        /// <remarks>
        /// Represents the list of headers for a message. Typically, the headers of a message
        /// will contain transmission headers such as From and To along with metadata headers
        /// such as Subject and Date, but may include just about anything.
        /// To access any MIME headers other than MimeKit.HeaderId.MimeVersion, you will
        /// need to access the MimeKit.MimeEntity.Headers property of the MimeKit.MimeMessage.Body.
        /// </remarks>
        public HeaderList Headers { get; }
        /// <summary>
        /// Gets the body parts of the message.
        /// </summary>
        /// <remarks>
        /// Traverses over the MIME tree, enumerating all of the MimeKit.MimeEntity objects,
        /// but does not traverse into the bodies of attached messages.
        /// </remarks>
        public IEnumerable<MimeEntity> BodyParts { get; }
        /// <summary>
        /// Gets the attachments.
        /// </summary>
        /// <remarks>
        /// Traverses over the MIME tree, enumerating all of the MimeKit.MimeEntity objects
        /// that have a Content-Disposition header set to "attachment".
        /// </remarks>
        public IEnumerable<MimeEntity> Attachments { get; }
    }
}
