using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using KoboRack.Core.IServices;
using KoboRack.Model.Entities;
using System.Security.Cryptography;
using System.Text;

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


        public string GenerateOtpEmailBody(string otpCode)
        {
            return $@"
                    <html>
                    <head>
                        <style>
                            .email-container {{
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                background-color: #f4f4f4;
                                padding: 40px;
                                color: #333;
                                max-width: 600px;
                                margin: auto;
                                border-radius: 10px;
                                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            }}

                            .email-header {{
                                text-align: center;
                                margin-bottom: 30px;
                            }}

                            .otp-code {{
                                display: inline-block;
                                padding: 15px 30px;
                                background-color: #007bff;
                                color: #fff;
                                font-size: 32px;
                                letter-spacing: 10px;
                                border-radius: 8px;
                                font-weight: bold;
                                margin-top: 20px;
                            }}

                            .email-body {{
                                font-size: 16px;
                                line-height: 1.6;
                                text-align: center;
                            }}

                            .footer {{
                                margin-top: 40px;
                                font-size: 12px;
                                color: #777;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <div class='email-header'>
                                <h2>Your Verification Code</h2>
                            </div>
                            <div class='email-body'>
                                <p>Use the 6-digit code below to verify your email address:</p>
                                <div class='otp-code'>{otpCode}</div>
                                <p>This code will expire in 10 minutes for your security.</p>
                                <p>If you didn't request this code, please ignore this email.</p>
                            </div>
                            <div class='footer'>
                                <p>&copy; 2025 Your Company. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>";
            

        }
    }

}
