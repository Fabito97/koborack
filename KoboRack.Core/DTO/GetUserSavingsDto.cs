using KoboRack.Model.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.DTO
{
    public class GetUserSavingsDto
    {
    
        public string Id { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime WithdrawalDate { get; set; }
        public string Purpose { get; set; }
        public decimal GoalAmount { get; set; }
        public decimal AmountSaved { get; set; }
        public decimal AmountToAdd { get; set; }
        public DateTime NextRuntime { get; set; }
        public decimal TargetAmount { get; set; }
        public FundFrequency FundFrequency { get; set; }
        public string TargetName { get; set; }
        public string GoalUrl { get; set; }
        public bool AutoSave { get; set; }
    
    }
}
