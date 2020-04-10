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
        /// <returns>An awaitable <seealso cref="Task"/> which represents this method call.</returns>
        public Task SendMail<T>(
            T data,
            MailboxAddress[] to,
            MailboxAddress[]? cc = default,
            MailboxAddress[]? bcc = default,
            MailboxAddress? from = default) where T : MailBase
        {
            from ??= _options.DefaultFromAddress ?? throw new ArgumentException(nameof(from), $"Either the parameter {nameof(from)} must be set or the {nameof(_options.DefaultFromAddress)} must be set.");
            if (to.Length == 0)
                throw new ArgumentException(nameof(to), $"There must be atleast one mail address in the {nameof(to)} parameter.");

            Action<T, MimeMessage> messageTransform = (d, m) =>
            {
                m.From.Add(from);
                m.To.AddRange(to);
                if (cc != default && cc.Any())
                    m.Cc.AddRange(cc);
                if (bcc != default && bcc.Any())
                    m.Bcc.AddRange(bcc);
            };

            return SendMail(data, messageTransform);
        }

        /// <summary>
        /// Sends the email
        /// </summary>
        /// <typeparam name="T">The type of the model carrying the payload of the mail.</typeparam>
        /// <param name="data">The message payload</param>
        /// <param name="transformMessage">An action that can be used to set the different properties on the <seealso cref="MimeMessage"/>. The To and From properties must be set.</param>
        /// <returns>An awaitable <seealso cref="Task"/> which represents this method call.</returns>
        public Task SendMail<T>(
            T data,
            Action<T, MimeMessage> transformMessage) where T : MailBase
        {
            Func<T, MimeMessage, Task> asyncTransform = (d, m) =>
            {
                transformMessage(d, m);
                return Task.CompletedTask;
            };

            return SendMail(data, asyncTransform);
        }

        /// <summary>
        /// Sends the email
        /// </summary>
        /// <typeparam name="T">The type of the model carrying the payload of the mail.</typeparam>
        /// <param name="data">The message payload</param>
        /// <param name="transformMessage">An async action that can be used to set the different properties on the <seealso cref="MimeMessage"/>. The To and From properties must be set.</param>
        /// <returns>An awaitable <seealso cref="Task"/> which represents this method call.</returns>
        public async Task SendMail<T>(
            T data,
            Func<T, MimeMessage, Task> transformMessage) where T : MailBase
        {
            var message = await _renderModelToMimeMessage(data);

            await transformMessage.Invoke(data, message);
            if (( message.To?.Count ?? 0 ) == 0)
                throw new ArgumentException($"The {nameof(message.To)} parameter must be set in the {nameof(transformMessage)} action.");
            if (( message.From?.Count ?? 0 ) == 0)
                throw new ArgumentException($"The {nameof(message.From)} parameter must be set in the {nameof(transformMessage)} action.");

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
