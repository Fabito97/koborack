using System.ComponentModel.DataAnnotations.Schema;

namespace KoboRack.Core.DTO
{
    public class WalletDto
    {
        public string Id { get; set; }
        public string WalletNumber { get; set; }
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public decimal AvailableBalance => Balance - LockedAmount; // Computed property for available balance
        public decimal LockedAmount { get; set; } // Funds committed to savings goals
        public string Reference { get; set; }
        public string PaystackCustomerCode { get; set; }
        public string UserId { get; set; }
    }
}
