using MailKit.Net.Smtp;
using MimeKit;
using SMTPClient.Configuration;

namespace SMTPClient.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(EmailConfiguration emailConfig, ILogger<EmailSender> logger)
        {
            _emailConfig = emailConfig;
            _logger = logger;
        }
        public async Task SendEmailAsync(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format(message.Content) };

            if (message.Attachments != null && message.Attachments.Any())
            {
                foreach (var attachment in message.Attachments)
                {
                    byte[] fileBytes;
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                await client.SendAsync(mailMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
                _logger.LogInformation("Mail sent successfully");
            }
        }
    }
}
