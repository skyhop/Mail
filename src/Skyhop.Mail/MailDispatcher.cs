using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using MimeKit;
using Skyhop.Mail.Abstractions;
using Skyhop.Mail.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Skyhop.Mail
{
    public class MailDispatcher
    {
        private readonly RazorViewToStringRenderer _renderer;
        private readonly MailDispatcherOptions _options;
        private readonly IMailSender _mailSender;

        public MailDispatcher(RazorViewToStringRenderer renderer, IOptions<MailDispatcherOptions> options, IMailSender mailSender)
        {
            _renderer = renderer;
            _options = options.Value;
            _mailSender = mailSender;
        }

        public async Task SendMail<T>(
            T data,
            MailboxAddress[] to,
            MailboxAddress[]? cc = default,
            MailboxAddress[]? bcc = default,
            MailboxAddress? from = default) where T : MailBase
        {
            from ??= _options.DefaultFromAddress ?? throw new ArgumentException(nameof(from), $"Either the parameter {nameof(from)} must be set or the {nameof(_options.DefaultFromAddress)} must be set.");
            if (to.Length == 0)
                throw new ArgumentException(nameof(to), $"There must be atleast one mail address in the {nameof(to)} parameter.");

            var message = await _fillMailMessage(data);

            message.From.Add(from);
            message.To.AddRange(to);
            if (cc != default && cc.Any())
                message.Cc.AddRange(cc);
            if (bcc != default && bcc.Any())
                message.Bcc.AddRange(bcc);

            await _mailSender.SendMail(message);
        }

        private async Task<MimeMessage> _fillMailMessage<T>(T data)
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
