using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using KoboRack.Core.IServices;
using KoboRack.Model.Entities;
using Microsoft.Extensions.Configuration;
using KoboRack.Data.Repositories.Interface;
using Microsoft.Extensions.Logging;

namespace KoboRack.Core.Services
{
    public class CloudinaryServices<T> : ICloudinaryServices<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<T> _logger;
        public CloudinaryServices(IGenericRepository<T> repository, IConfiguration configuration, ILogger<T> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger=logger ?? throw new ArgumentNullException(nameof(logger));

            var cloudinarySettings = configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>(); 

            _cloudinary = new Cloudinary(new Account(
                cloudinarySettings.CloudName,
                cloudinarySettings.ApiKey,
                cloudinarySettings.ApiSecret
            ));
        }
        public async Task<string> UploadImage(string entityId, IFormFile file)
        {
            var entity = _repository.GetById(entityId);

            if (entity == null)
            {
                return $"{typeof(T).Name} not found";
            }          
            var upload = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };
            var uploadResult = await _cloudinary.UploadAsync(upload);
            _repository.UpdateAsync(entity);
            try
            {
                await _repository.SaveChangesAsync();
                return uploadResult.SecureUrl.AbsoluteUri;
            }
            catch (Exception ex)
            {
                _logger.LogError("Database update failed: {Message}", ex.Message);
                return "Database update error occurred";
            }
        }
    }

}
