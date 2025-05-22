using MimeKit;
using KoboRack.Model.Entities;
namespace KoboRack.Core.IServices
{
    public interface IEmailServices
    {
        Task SendHtmlEmailAsync(MailRequest mailRequest);
        void AttachFile(BodyBuilder builder, string filePath, string fileName);
    }
}
