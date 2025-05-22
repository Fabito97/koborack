using KoboRack.Model.Enums;

namespace KoboRack.Core.DTO
{
    public class KycResponseDto
    {
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public Occupation Occupation { get; set; }
        public string Address { get; set; } = string.Empty;
        public string BVN { get; set; } = string.Empty;
        public IdentificationType IdentificationType { get; set; }
        public string IdentificationNumber { get; set; }
        public string IdentificationDocumentUrl { get; set; }
        public string ProofOfAddressUrl { get; set; }
    }
}
