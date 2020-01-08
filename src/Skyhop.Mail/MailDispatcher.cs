using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Linq;
using System.Threading.Tasks;

namespace Skyhop.Mail
{
    public class MailDispatcher
    {
        private readonly RazorViewToStringRenderer _renderer;
        private readonly MailDispatcherOptions _options;

        public MailDispatcher(RazorViewToStringRenderer renderer, IOptions<MailDispatcherOptions> options)
        {
            _renderer = renderer;
            _options = options.Value;
        }

        public async Task SendMail<T>(
            T data,
            MailboxAddress? from = default,
            MailboxAddress[]? to = default,
            MailboxAddress[]? cc = default,
            MailboxAddress[]? bcc = default,
            MimeEntity[]? attachments = default) where T : MailBase
        {
            var message = await _fillMailMessage(data, attachments);

            if (from != default || _options.DefaultFromAddress != default) message.From.Add(from ?? _options.DefaultFromAddress);

            if (to != default && to.Any()) message.To.AddRange(to);
            if (cc != default && cc.Any()) message.Cc.AddRange(cc);
            if (bcc != default && bcc.Any()) message.Bcc.AddRange(bcc);

            if(_options.MailSender != null)
                await _options.MailSender.Invoke(message);
        }

        private async Task<MimeMessage> _fillMailMessage<T>(T data, MimeEntity[]? attachments = default)
             where T : MailBase
        {
            var (htmlBody, textBody) = await _getBody(data);

            data.BodyBuilder.HtmlBody = htmlBody;
            data.BodyBuilder.TextBody = textBody;

            if (attachments != default && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    data.BodyBuilder.Attachments.Add(attachment);
                }
            }

            data.MailMessage.Body = data.BodyBuilder.ToMessageBody();

            return data.MailMessage;
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
