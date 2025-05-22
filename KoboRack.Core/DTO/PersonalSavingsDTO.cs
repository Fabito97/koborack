using Microsoft.AspNetCore.Http;
using KoboRack.Model.Enums;

namespace KoboRack.Core.DTO
{
    public class PersonalSavingsDTO
    {
        public DateTime EndDate { get; set; }
        public DateTime WithdrawalDate { get; set; }
        public decimal AmountToAdd { get; set; }
        public DateTime NextRuntime { get; set; }
        public decimal TargetAmount { get; set; }
        public FundFrequency FundFrequency { get; set; }
       
        public string TargetName { get; set; }
        public IFormFile GoalUrl { get; set; }
        public bool AutoSave { get; set; }
        public string UserId { get; set; }
    }
}
