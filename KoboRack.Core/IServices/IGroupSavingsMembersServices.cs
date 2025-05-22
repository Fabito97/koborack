using KoboRack.Core.DTO;

namespace KoboRack.Core.IServices
{
    public interface IGroupSavingsMembersServices
    {
        Task<ResponseDto<bool>> JoinGroupSavingsAsync(string userId, string groupId);
        Task<ResponseDto<Dictionary<string, string>>> GetGroupMembersAsync(string groupId);
    }
}
