using KoboRack.Model.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoboRack.Model.Entities
{
    public class Kyc : BaseEntity
    {
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public Occupation Occupation { get; set; } 
        public string Address { get; set; } = string.Empty;
        public string BVN { get; set; } = string.Empty;
        public IdentificationType IdentificationType { get; set; }
        public string IdentificationNumber { get; set; } = string.Empty;
        public string IdentificationDocumentUrl { get; set; }
        public string ProofOfAddressUrl { get; set; } = string.Empty;

        [ForeignKey("AppUserId")]
        public string AppUserId { get; set; }
    }
}
