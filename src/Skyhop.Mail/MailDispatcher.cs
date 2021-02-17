using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;
using Skyhop.Mail.Abstractions;
using Skyhop.Mail.Internal;
using Skyhop.Mail.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Skyhop.Mail
{
    /// <inheritdoc/>
    public class MailDispatcher : IMailDispatcher
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

        /// <inheritdoc />
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

            Func<IMessageSenderInfo, Task> senderTransform = message =>
            {
                message.From.Add(from);
                message.To.AddRange(to);
                if (cc != default && cc.Any())
                    message.Cc.AddRange(cc);
                if (bcc != default && bcc.Any())
                    message.Bcc.AddRange(bcc);

                return Task.CompletedTask;
            };

            return SendMail(data, senderTransform);
        }

        /// <inheritdoc />
        public async Task SendMail<T>(T data, Func<IMessageSenderInfo, Task> senderTransformation)
            where T : MailBase
        {
            _ = senderTransformation ?? throw new ArgumentNullException(nameof(senderTransformation));

            var message = await _renderModelToMimeMessage(data);

            if (data.MessageTransform != null)
                await data.MessageTransform(message);

            await senderTransformation(message);

            if (( message.To?.Count ?? 0 ) == 0)
                throw new ArgumentException($"The {nameof(message.To)} parameter must be set in the {nameof(senderTransformation)} action.");
            if (( message.From?.Count ?? 0 ) == 0)
                throw new ArgumentException($"The {nameof(message.From)} parameter must be set in the {nameof(senderTransformation)} action.");

            using var scope = _scopeFactory.CreateScope();
            var mailSender = scope.ServiceProvider.GetRequiredService<IMailSender>();
            await mailSender.SendMail(message.WrappedMessage);
        }

        private async Task<MimeMessageWrapper> _renderModelToMimeMessage<T>(T data)
             where T : MailBase
        {
            var (htmlBody, textBody) = await _getBody(data);

            data.BodyBuilder.HtmlBody = htmlBody;
            data.BodyBuilder.TextBody = textBody;

            var mimeMessage = new MimeMessage()
            {
                Body = data.BodyBuilder.ToMessageBody(),
                Subject = data.Subject,
                Priority = data.Priority,
                XPriority = data.XPriority,
                Importance = data.Importance
            };

            return new MimeMessageWrapper(mimeMessage);
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
