using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Interface
{
    public interface IGroupTransactionRepository : IGenericRepository<GroupTransaction>
    {
        List<GroupTransaction> GetGroupTransactionsAsync();
        Task AddGroupTransactionAsync(GroupTransaction groupTransaction);
        Task DeleteGroupTransactionAsync(GroupTransaction groupTransaction);
        Task DeleteAllGroupTransactionAsync(List<GroupTransaction> groupTransactions);
        void UpdateGroupTransactionAsync(GroupTransaction groupTransaction);
        List<GroupTransaction> FindGroupTransactions(Expression<Func<GroupTransaction, bool>> expression);
        Task<GroupTransaction> GetGroupTransactionByIdAsync(string id);
    }
}
