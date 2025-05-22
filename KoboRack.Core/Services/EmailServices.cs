using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using KoboRack.Core.IServices;
using KoboRack.Model.Entities;

namespace KoboRack.Core.Services
{
    public class EmailServices : IEmailServices
    {
            private readonly EmailSettings emailSettings;
            public EmailServices(IOptions<EmailSettings> options)
            {
                this.emailSettings = options.Value;
            }
            public async Task SendHtmlEmailAsync(MailRequest mailRequest)
            {
                var message = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(emailSettings.Email)
                };
                message.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                message.Subject = mailRequest.Subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = mailRequest.Body
                };

                try
                {
                    AttachFile(builder, "EmailAttachments/freedom.jpeg", "attachment1.jpeg");
                    AttachFile(builder, "EmailAttachments/lorem.jpeg", "attachment2.jpeg");

                    message.Body = builder.ToMessageBody();

                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.SslOnConnect);
                        await client.AuthenticateAsync(emailSettings.Email, emailSettings.Password);
                        await client.SendAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    throw new InvalidOperationException("Email sending failed.", ex);
                }
            }
            public void AttachFile(BodyBuilder builder, string filePath, string fileName)
            {
                if (System.IO.File.Exists(filePath))
                {
                    using (var ms = new MemoryStream())
                    using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        builder.Attachments.Add(fileName, fileBytes, ContentType.Parse("application/octet-stream"));
                    }
                }
            }
    }
}
