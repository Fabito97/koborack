using KoboRack.Model.Enums;

namespace KoboRack.Model.Entities
{
    public class Group : BaseEntity
    {
        public string UserId {  get; set; } 
        public string SaveName { get; set; } 
        public string Description { get; set; }
        public string Avatar { get; set; } 
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; }
        public decimal ContributionAmount { get; set; }  
        public bool IsOpen { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public DateTime ExpectedStartDate { get; set; }
        public int MemberCount { get; set; }
        public DateTime RunTime {  get; set; }
        public GroupStatus GroupStatus { get; set; }  
        public string SafePortraitImageURL { get; set; }
        public string TermsAndCondition { get; set; }
        public DateTime NextRunTime { get; set; }
        public string PurposeAndGoal {  get; set; }
        public DateTime CashoutDate { get; set; }
        public DateTime NextDueDate { get; set; }
        public int MaxNumberOfParticipants { get; set; }
        public FundFrequency Schedule { get; set; } 

        public ICollection<GroupTransaction> GroupTransactions { get; set; }
        public ICollection<AppUser> AppUsers { get; set; }
    }
}
