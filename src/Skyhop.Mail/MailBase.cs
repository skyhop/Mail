using MimeKit;
using System;
using System.Collections.Generic;

namespace Skyhop.Mail
{
    public class MailBase
    {
        public MailBase()
            : this(new BodyBuilder())
        { }

        public MailBase(BodyBuilder bodyBuilder)
        {
            BodyBuilder = bodyBuilder;
        }

        public MessageImportance Importance { get; set; } = MessageImportance.Normal;
        public MessagePriority Priority { get; set; } = MessagePriority.Normal;
        public XMessagePriority XPriority { get; set; } = XMessagePriority.Normal;
        public string Subject { get; set; } = "";
        public AttachmentCollection Attachments
            => BodyBuilder.Attachments;
        public AttachmentCollection LinkedResources
            => BodyBuilder.LinkedResources;

        protected internal BodyBuilder BodyBuilder { get; }
    }
}
