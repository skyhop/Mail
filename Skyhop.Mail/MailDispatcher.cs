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

        public async Task<MimeMessage> GenerateMessage<T>(T data) where T : MailBase
        {
            var body = await _renderer.GetViewForModel(data);

            body = PreMailer.Net.PreMailer
                .MoveCssInline(body)
                .Html;

            data.BodyBuilder.HtmlBody = body;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(body);

            data.BodyBuilder.TextBody = String.Join(" ", doc.DocumentNode.SelectNodes("//text()").Select(q => q.InnerText));

            var message = new MimeMessage();

            message.Body = data.BodyBuilder.ToMessageBody();
            message.From.Add(_options.DefaultFromAddress);

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
            var body = await _renderer.GetViewForModel(data);

            body = PreMailer.Net.PreMailer
                .MoveCssInline(body)
                .Html;

            data.BodyBuilder.HtmlBody = body;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(body);

            data.BodyBuilder.TextBody = string.Join(" ", doc.DocumentNode.SelectNodes("//text()").Select(q => q.InnerText));

            if (attachments != default && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    data.BodyBuilder.Attachments.Add(attachment);
                }
            }

            data.MailMessage.Body = data.BodyBuilder.ToMessageBody();

            if (from != default || _options.DefaultFromAddress != default) data.MailMessage.From.Add(from ?? _options.DefaultFromAddress);

            if (to != default && to.Any()) data.MailMessage.To.AddRange(to);
            if (cc != default && cc.Any()) data.MailMessage.Cc.AddRange(cc);
            if (bcc != default && bcc.Any()) data.MailMessage.Bcc.AddRange(bcc);

            _options.MailSender.Invoke(data.MailMessage);
        }
    }
}
