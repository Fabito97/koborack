using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Implementation
{
    public class AppUserTransactionRepository : GenericRepository<AppUserTransaction>, IAppUserTransactionRepository
    {
        public AppUserTransactionRepository(SaviDbContext context) : base(context)
        {
        }

        public async Task AddAppUserTransactionAsync(AppUserTransaction appUserTransaction)
        {
            await AddAsync(appUserTransaction);
        }

        public async Task DeleteAllAppUserTransactionAsync(List<AppUserTransaction> appUserTransactions)
        {
            await DeleteAllAsync(appUserTransactions);
        }

        public async Task DeleteAppUserTransactionAsync(AppUserTransaction appUserTransaction)
        {
            await DeleteAsync(appUserTransaction);
        }

        public List<AppUserTransaction> FindAppUserTransactions(Expression<Func<AppUserTransaction, bool>> expression)
        {
            return FindAsync(expression);
        }
        public async Task<AppUserTransaction> GetAppUserTransactionByIdAsync(string id)
        {
            return  await GetByIdAsync(id);
        }

        public List<AppUserTransaction> GetAppUserTransactionsAsync()
        {
            return GetAll();
        }

        public void UpdateAppUserTransactionAsync(AppUserTransaction appUserTransaction)
        {
            UpdateAsync(appUserTransaction);
        }
    }
}
