using Microsoft.AspNetCore.Http;
using KoboRack.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace KoboRack.Core.DTO
{
    public class KycRequestDto
    {
        [Required(ErrorMessage = "Date ofBirth is required")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [EnumDataType(typeof(Gender), ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        [Required]
        [EnumDataType(typeof(Occupation), ErrorMessage = "Occupation is required")]
        public Occupation Occupation { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "BVN is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "The BVN must be 11 characters long.")]
        public string BVN { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(IdentificationType), ErrorMessage = "Identification Type is required")]
        public IdentificationType IdentificationType { get; set; }

        [Required(ErrorMessage = "Identification Number is required")]
        public string IdentificationNumber { get; set; }

        [Required(ErrorMessage = "Identification Document is required")]
        public IFormFile IdentificationDocumentUrl { get; set; }

        [Required(ErrorMessage = "Proof of Address is required")]
        public IFormFile ProofOfAddressUrl { get; set; } 
    }
}
