using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Interface
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        List<Group> GetGroupsAsync();
        List<Group> GetGroups(Expression<Func<Group, bool>> expression);
        Task<bool> CreateGroup(Group group);
        Task AddGroupAsync(Group group);
        Task<Group> AddGroupAsync2(Group group);
        Task DeleteGroupAsync(Group group);
        Task DeleteAllGroupAsync(List<Group> groups);
        void UpdateGroupAsync(Group group);
        List<Group> FindGroups(Expression<Func<Group, bool>> expression);
        Task<Group> GetGroupByIdAsync(string id);
    }
}
