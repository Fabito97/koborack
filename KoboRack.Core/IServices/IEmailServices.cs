using MimeKit;
using KoboRack.Model.Commons;
namespace KoboRack.Core.IServices
{
    public interface IEmailServices
    {
        Task SendHtmlEmailAsync(MailRequest mailRequest);
        void AttachFile(BodyBuilder builder, string filePath, string fileName);

        string GenerateOtpEmailBody(string otpCode);
    }
}
