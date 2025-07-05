using KoboRack.Model.Commons;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoboRack.Model.Entities
{
    public class Wallet : BaseEntity
    {
        public string WalletNumber { get; set; } 
        public string Currency { get; set; } 
        public decimal Balance { get; set; }
        public string Reference { get; set; }

        //public decimal LockedAmount { get; set; } // Funds committed to savings goals
        //public decimal PendingAmount { get; set; } // Incoming but unconfirmed funds

        //public decimal AvailableBalance => Balance - LockedAmount - PendingAmount;

        public string PaystackCustomerCode { get; set; }
        //public string? VirtualAccountNumber { get; set; }
        //public string? VirtualBankName { get; set; }
        //public string? VirtualAccountName { get; set; }
        //public string? DvaReferenceId { get; set; }

        public string? TransactionPin { get; set; }

        [ForeignKey("AppUserId")]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public ICollection<WalletFunding> WalletFundings { get; set; }

    }
}
