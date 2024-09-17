using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.Shared.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HomelessAnimals.BusinessLogic.EmailSender
{
    public class GmailEmailSender : IEmailSender
    {
        private readonly GmailSettings _options;
        private readonly ILogger<GmailEmailSender> _logger;

        public GmailEmailSender(IOptions<GmailSettings> options, ILogger<GmailEmailSender> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task Send(string recipient, string subject, string message)
        {
            try
            {
                var mail = new MimeMessage();

                mail.From.Add(new MailboxAddress(_options.Sender, _options.Account));
                mail.Sender = new MailboxAddress(_options.Sender, _options.Account);

                mail.To.Add(new MailboxAddress(string.Empty, recipient));

                var body = new BodyBuilder();
                mail.Subject = subject;
                body.HtmlBody = message;
                mail.Body = body.ToMessageBody();

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(_options.Account, _options.Password);
                await smtp.SendAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Email to {Email} could not be sent", recipient);
                throw;
            }
        }
    }
}
