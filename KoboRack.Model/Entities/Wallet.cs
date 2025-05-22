using System.ComponentModel.DataAnnotations.Schema;

namespace KoboRack.Model.Entities
{
    public class Wallet : BaseEntity
    {
        public string WalletNumber { get; set; } 
        public string Currency { get; set; } 
        public decimal Balance { get; set; }
        public string Reference { get; set; } 
        public string PaystackCustomerCode { get; set; } 
        [ForeignKey("AppUserId")]
        public string AppUserId { get; set; }
        public string TransactionPin { get; set; }  
        public ICollection<WalletFunding> WalletFundings { get; set; }
    }
}
