using KoboRack.Model.Commons;

namespace KoboRack.Model.Entities
{
    public class FundingAnalytics : BaseEntity
    {
        //public decimal WalletBalance { get; set; }
        public decimal Savings { get; set; }
        public decimal Withdrawal { get; set; }
        public decimal Contribution { get; set; }
        //public decimal TotalSavings { get; set; }

    }
}
