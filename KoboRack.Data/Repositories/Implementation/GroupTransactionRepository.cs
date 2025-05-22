using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Implementation
{
    public class GroupTransactionRepository : GenericRepository<GroupTransaction>, IGroupTransactionRepository
    {
        public GroupTransactionRepository(SaviDbContext context) : base(context)
        {
        }

        public async Task AddGroupTransactionAsync(GroupTransaction groupTransaction)
        {
            await AddAsync(groupTransaction);
        }

        public async Task DeleteAllGroupTransactionAsync(List<GroupTransaction> groupTransactions)
        {
            await DeleteAllAsync(groupTransactions);
        }

        public async Task DeleteGroupTransactionAsync(GroupTransaction groupTransaction)
        {
           await DeleteAsync(groupTransaction);
        }

        public List<GroupTransaction> FindGroupTransactions(Expression<Func<GroupTransaction, bool>> expression)
        {
            return FindAsync(expression);
        }
        public async Task<GroupTransaction> GetGroupTransactionByIdAsync(string id)
        {
            return await GetByIdAsync(id);
        }

        public List<GroupTransaction> GetGroupTransactionsAsync()
        {
            return GetAll();
        }

        public void UpdateGroupTransactionAsync(GroupTransaction groupTransaction)
        {
            UpdateAsync(groupTransaction);
        }
    }
}
