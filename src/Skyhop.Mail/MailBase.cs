using MimeKit;

namespace Skyhop.Mail
{
    /// <summary>
    /// Base class for all view-models for the <seealso cref="MailDispatcher"/> views
    /// </summary>
    public class MailBase
    {
        /// <summary>
        /// Constructor for <see cref="MailBase"/>
        /// </summary>
        public MailBase()
            : this(new BodyBuilder())
        { }

        /// <summary>
        /// Constructor for <see cref="MailBase"/> with option to specify a <seealso cref="MimeKit.BodyBuilder"/>
        /// </summary>
        /// <param name="bodyBuilder"></param>
        public MailBase(BodyBuilder bodyBuilder)
        {
            BodyBuilder = bodyBuilder;
        }

        /// <summary>
        /// Get or set the value of the Importance header.
        /// </summary>
        public MessageImportance Importance { get; set; } = MessageImportance.Normal;

        /// <summary>
        /// Get or set the value of the Priority header.
        /// </summary>
        public MessagePriority Priority { get; set; } = MessagePriority.Normal;

        /// <summary>
        /// Get or set the value of the X-Priority header.
        /// </summary>
        public XMessagePriority XPriority { get; set; } = XMessagePriority.Normal;

        /// <summary>
        /// Gets or sets the subject of the message.
        /// </summary>
        public string Subject { get; set; } = "";

        /// <summary>
        /// Represents a collection of file attachments that will be included in the message.
        /// </summary>
        public AttachmentCollection Attachments
            => BodyBuilder.Attachments;

        /// <summary>
        /// Linked resources are a special type of attachment which are linked to from the HTML body
        /// </summary>
        public AttachmentCollection LinkedResources
            => BodyBuilder.LinkedResources;

        /// <summary>
        /// The body builder used for the <see cref="Attachments"/>, <see cref="LinkedResources"/>, Html body and text body of the message.
        /// </summary>
        protected internal BodyBuilder BodyBuilder { get; }
    }
}
