using Microsoft.AspNetCore.Http;
using KoboRack.Model.Enums;

namespace KoboRack.Core.DTO
{
    public class GroupDTO2
    {
        public string UserId { get; set; }
        public string SaveName { get; set; }
        public string Description { get; set; }
        public decimal ContributionAmount { get; set; }
        // public DateTime ExpectedEndDate { get; set; }
        // public DateTime ExpectedStartDate { get; set; }
        //public int MemberCount { get; set; }
        // public DateTime RunTime { get; set; }
        //public GroupStatus GroupStatus { get; set; }
        //public string SafePortraitImageURL { get; set; }
        //public int MaxNumberOfParticipants { get; set; }
        public string TermsAndCondition { get; set; }
        //public DateTime NextRunTime { get; set; }
        public string PurposeAndGoal { get; set; }
        public FundFrequency Schedule { get; set; }

        public IFormFile FormFile { get; set; }
    }
}
