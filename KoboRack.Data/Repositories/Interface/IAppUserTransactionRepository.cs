using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Interface
{
    public interface IAppUserTransactionRepository : IGenericRepository<AppUserTransaction>
    {
        List<AppUserTransaction> GetAppUserTransactionsAsync();
        Task AddAppUserTransactionAsync(AppUserTransaction appUserTransaction);
        Task DeleteAppUserTransactionAsync(AppUserTransaction appUserTransaction);
        Task DeleteAllAppUserTransactionAsync(List<AppUserTransaction> appUserTransactions);
        void UpdateAppUserTransactionAsync(AppUserTransaction appUserTransaction);
        List<AppUserTransaction> FindAppUserTransactions(Expression<Func<AppUserTransaction, bool>> expression);
        Task<AppUserTransaction> GetAppUserTransactionByIdAsync(string id);
    }
}
