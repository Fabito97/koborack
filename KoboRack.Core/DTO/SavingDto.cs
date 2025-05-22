
using Microsoft.AspNetCore.Http;

namespace KoboRack.Core.DTO
{
    public class SavingDto
    {
        public string UserId { get; set; }
        public IFormFile Goalurl { get; set; }
        public bool AutoSaving { get; set; }
    }
}
