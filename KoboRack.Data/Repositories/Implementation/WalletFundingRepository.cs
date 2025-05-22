using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Implementation
{
    public class WalletFundingRepository : GenericRepository<WalletFunding>, IWalletFundingRepository
    {
        public WalletFundingRepository(SaviDbContext context) : base(context)
        {
        }

        public async Task AddWalletFundingAsync(WalletFunding walletFunding)
        {
           await AddAsync(walletFunding);
        }

        public async Task DeleteAllWalletFundingAsync(List<WalletFunding> walletFundings)
        {
            await DeleteAllAsync(walletFundings);
        }

        public async Task DeleteWalletFundingAsync(WalletFunding walletFunding)
        {
           await DeleteAsync(walletFunding);
        }

        public List<WalletFunding> FindWalletFundings(Expression<Func<WalletFunding, bool>> expression)
        {
            return FindAsync(expression).ToList();
        }

        public async Task<WalletFunding> GetWalletFundingByIdAsync(string id)
        {
            return await GetByIdAsync(id);
        }

        public List<WalletFunding> GetWalletFundingsAsync()
        {
            return GetAll();
        }

        public void UpdateWalletFundingAsync(WalletFunding walletFunding)
        {
            UpdateAsync(walletFunding);
        }
    }
}
