using KoboRack.Data.Repository.DTO;
using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Interface
{
    public interface IGroupSavingsMembersRepository
    {
        Task<List<GroupMembersDto2>> GetListOfGroupMembersAsync(string UserId);
        Task<bool> CheckIfUserExist(string UserId, string GroupId);
        Task<ResponseDto2<AppUserDto2>> GetUserByIdAsync(string UserId);
        Task<int> GetGroupLastUserPosition(string GroupId);
        Task<bool> CreateSavingsGroupMembersAsync(GroupSavingsMembers groupSavingsMembers);
        List<GroupSavingsMembers> FindAsync(Expression<Func<GroupSavingsMembers, bool>> expression);
        Task<GroupSavingsMembers> FindAsync2(Expression<Func<GroupSavingsMembers, bool>> expression);
        Task<int> GetNextPositionToPay(string GroupId);
        void UpdateGroupSavingsMember(GroupSavingsMembers groupSavingMembers);
        Task<int> GetTotalPositionsInGroup(string groupId);
    }
}
