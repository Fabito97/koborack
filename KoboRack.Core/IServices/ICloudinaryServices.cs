using Microsoft.AspNetCore.Http;

namespace KoboRack.Core.IServices
{
    public interface ICloudinaryServices<T> where T : class
    {
        Task<string> UploadImage(string entityId, IFormFile file);
    }
}
