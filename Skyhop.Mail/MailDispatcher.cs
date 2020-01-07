using HtmlAgilityPack;
using MimeKit;
using SkyHop.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Skyhop.Mail
{
    public class MailDispatcher
    {
        private readonly RazorViewToStringRenderer _renderer;
        private readonly MailDispatcherOptions _options;

        public MailDispatcher(RazorViewToStringRenderer renderer, MailDispatcherOptions options)
        {
            _renderer = renderer;
            _options = options;
        }

        public async Task<MimeMessage> GenerateMessage<T>(T data, MimeEntity[] attachments = default) where T : MailBase
        {
            var (htmlBody, textBody) = await GetBody(data);

            data.BodyBuilder.HtmlBody = htmlBody;
            data.BodyBuilder.TextBody = textBody;

            if (attachments != default && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    data.BodyBuilder.Attachments.Add(attachment);
                }
            }

            var message = new MimeMessage
            {
                Body = data.BodyBuilder.ToMessageBody()
            };

            return message;
        }

        public async Task SendMail<T>(
            T data,
            MailboxAddress from = default,
            MailboxAddress[] to = default,
            MailboxAddress[] cc = default,
            MailboxAddress[] bcc = default,
            MimeEntity[] attachments = default) where T : MailBase
        {
            var message = await GenerateMessage(data, attachments);

            if (from != default || _options.DefaultFromAddress != default) data.MailMessage.From.Add(from ?? _options.DefaultFromAddress);

            if (to != default && to.Any()) data.MailMessage.To.AddRange(to);
            if (cc != default && cc.Any()) data.MailMessage.Cc.AddRange(cc);
            if (bcc != default && bcc.Any()) data.MailMessage.Bcc.AddRange(bcc);

            _options.MailSender.Invoke(data.MailMessage);
        }

        private async Task<(string HtmlBody, string TextBody)> GetBody<T>(T data)
        {
            var htmlBody = await _renderer.GetViewForModel(data);

            htmlBody = PreMailer.Net.PreMailer
                .MoveCssInline(htmlBody)
                .Html;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlBody);

            var txtBody = String.Join(" ", doc.DocumentNode.SelectNodes("//text()").Select(q => q.InnerText));

            return (htmlBody, txtBody);
        }
    }
}
