using KoboRack.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.DTO
{
    public class GroupDTO
    {
        public string Id { get; set; }
        public string SaveName { get; set; } 
        public string Description { get; set; }
        public decimal ContributionAmount { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public DateTime ExpectedStartDate { get; set; }
        public int MemberCount { get; set; }
        public DateTime RunTime { get; set; }
        public DateTime ActualStartDate { get; set; }
        public GroupStatus GroupStatus { get; set; }
        public string SafePortraitImageURL { get; set; }
        public string TermsAndCondition { get; set; }
        public DateTime NextRunTime { get; set; }
        public string PurposeAndGoal { get; set; }
        public FundFrequency Schedule { get; set; }


    }
}
