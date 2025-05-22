using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Implementation
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        public GroupRepository(SaviDbContext context) : base(context)
        {
        }

        public async Task AddGroupAsync(Group group)
        {
            await AddAsync(group);
        }
        public async Task<Group> AddGroupAsync2(Group group)
        {
            return  AddAsync2(group);
        }

        public async Task DeleteAllGroupAsync(List<Group> groups)
        {
           await DeleteAllAsync(groups);
        }

        public async Task DeleteGroupAsync(Group group)
        {
           await DeleteAsync(group);
        }

        public List<Group> FindGroups(Expression<Func<Group, bool>> expression)
        {
            return FindAsync(expression);
        }
        public async Task<Group> GetGroupByIdAsync(string id)
        {
            return await GetByIdAsync(id);
        }

        public List<Group> GetGroupsAsync()
        {
            return GetAll();
        }
        public List<Group> GetGroups(Expression<Func<Group, bool>> expression)
        {
            return GetAll(expression);
        }

        public void UpdateGroupAsync(Group group)
        {
            UpdateAsync(group);
        }
        public async Task<bool> CreateGroup(Group group)
        {
            var groups = await CreateAsync(group);
            if (!groups)
            {
                return false;
            }
            return true;
        }
    }
}
