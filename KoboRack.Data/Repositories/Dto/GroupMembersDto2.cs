namespace KoboRack.Data.Repository.DTO
{
    public class GroupMembersDto2
    {
        public AppUserDto2 User { get; set; }
        public string UserId { get; set; }
        public string GroupSavingsId { get; set; }
        public bool IsGroupOwner { get; set; }
        public int Positions { get; set; }
        public DateTime LastsavingsDate { get; set; }
    }
}
