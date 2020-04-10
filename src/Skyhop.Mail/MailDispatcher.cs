using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;
using Skyhop.Mail.Abstractions;
using Skyhop.Mail.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Skyhop.Mail
{
    /// <summary>
    /// Class used to generate and send emails
    /// </summary>
    public class MailDispatcher
    {
        private readonly RazorViewToStringRenderer _renderer;
        private readonly MailDispatcherOptions _options;
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// Constructor for <see cref="MailDispatcher"/>
        /// </summary>
        /// <param name="renderer">The renderer used to render the Razor view to html</param>
        /// <param name="options">The <see cref="MailDispatcherOptions"/> used to further setup the <see cref="MailDispatcher"/></param>
        /// <param name="scopeFactory">Factory used to create a scope to resolve the <see cref="IMailSender"/></param>
        public MailDispatcher(RazorViewToStringRenderer renderer, IOptions<MailDispatcherOptions> options, IServiceScopeFactory scopeFactory)
        {
            _renderer = renderer;
            _options = options.Value;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Sends the email
        /// </summary>
        /// <typeparam name="T">The type of the model carrying the payload of the mail.</typeparam>
        /// <param name="data">The message payload</param>
        /// <param name="to">The addresses to which the mail must be sent</param>
        /// <param name="cc">The addresses to which the mail must be cc'ed.</param>
        /// <param name="bcc">The addresses to which the mail must be bcc'ed.</param>
        /// <param name="from">The addresses from which the mail is sent, can be null, but then a DefaultFromAddress in <seealso cref="MailDispatcherOptions"/> must be set.</param>
        /// <param name="replyTo">Adds the Reply-To header to the message showing the preferred addresses to which a reply should be sent.</param>
        /// <returns>An awaitable <seealso cref="Task"/> which represents this method call.</returns>
        public async Task SendMail<T>(
            T data,
            MailboxAddress[] to,
            MailboxAddress[]? cc = default,
            MailboxAddress[]? bcc = default,
            MailboxAddress? from = default,
            MailboxAddress[]? replyTo = default) where T : MailBase
        {
            from ??= _options.DefaultFromAddress ?? throw new ArgumentException(nameof(from), $"Either the parameter {nameof(from)} must be set or the {nameof(_options.DefaultFromAddress)} must be set.");
            if (to.Length == 0)
                throw new ArgumentException(nameof(to), $"There must be atleast one mail address in the {nameof(to)} parameter.");

            var message = await _renderModelToMimeMessage(data);

            message.From.Add(from);
            message.To.AddRange(to);
            if (cc != default && cc.Any())
                message.Cc.AddRange(cc);
            if (bcc != default && bcc.Any())
                message.Bcc.AddRange(bcc);
            if (replyTo != default && replyTo.Any())
                message.ReplyTo.AddRange(replyTo);

            using var scope = _scopeFactory.CreateScope();
            var mailSender = scope.ServiceProvider.GetRequiredService<IMailSender>();
            await mailSender.SendMail(message);
        }

        private async Task<MimeMessage> _renderModelToMimeMessage<T>(T data)
             where T : MailBase
        {
            var (htmlBody, textBody) = await _getBody(data);

            data.BodyBuilder.HtmlBody = htmlBody;
            data.BodyBuilder.TextBody = textBody;

            return new MimeMessage()
            {
                Body = data.BodyBuilder.ToMessageBody(),
                Subject = data.Subject,
                Priority = data.Priority,
                XPriority = data.XPriority,
                Importance = data.Importance
            };
        }

        private async Task<(string HtmlBody, string TextBody)> _getBody<T>(T data)
        {
            var htmlBody = await _renderer.RenderViewForModel(data);

            htmlBody = PreMailer.Net.PreMailer
                .MoveCssInline(htmlBody)
                .Html;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlBody);

            var txtBody = string.Join(" ", doc.DocumentNode.SelectNodes("//text()").Select(q => q.InnerText));

            return (htmlBody, txtBody);
        }
    }
}
