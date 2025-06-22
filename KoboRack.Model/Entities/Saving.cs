using KoboRack.Model.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoboRack.Model.Entities
{
    public class Saving : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal GoalAmount { get; set; }
        public decimal AmountSaved { get; set; }
        public bool AutoSave { get; set; } = false;
        public string GoalUrl { get; set; }
        public string Purpose { get; set; } = string.Empty;
        
        public string Avatar { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime WithdrawalDate { get; set; }
        public DateTime? NextRuntime { get; set; }
        public string TargetName { get; set; }
        public decimal TargetAmount { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public decimal AmountToAdd { get; set; }
        public FundFrequency FundFrequency { get; set; }

        [ForeignKey("WalletId")]
        public string WalletId { get; set; }
    }
}
