using Microsoft.AspNetCore.Http;

namespace KoboRack.Core.DTO
{
    public class AppUserUpdateDto
    {
        public string UserId { get; set; }
        //public string Email { get; set; }
        public IFormFile formFile { get; set; }
    }
}
