using KoboRack.Model.Commons;
using KoboRack.Model.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoboRack.Model.Entities
{
    public class AppUserTransaction : BaseEntity
    {
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
        public TransactionStatus Status { get; set; }

        [ForeignKey("AppUserId")]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        [ForeignKey("WalletId")]
        public string WalletId { get; set; }
        public Wallet Wallet { get; set; }

    }
}
