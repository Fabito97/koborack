using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.DTO
{
    public class ResendVerificationOtpDto
    {
        [Required]
        public string UserId { get; set; }
    }
}
