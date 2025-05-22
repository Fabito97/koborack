using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Interface
{
    public interface IWalletFundingRepository : IGenericRepository<WalletFunding>
    {
        List<WalletFunding> GetWalletFundingsAsync();
        Task AddWalletFundingAsync(WalletFunding walletFunding);
        Task DeleteWalletFundingAsync(WalletFunding walletFunding);
        Task DeleteAllWalletFundingAsync(List<WalletFunding> walletFundings);
        void UpdateWalletFundingAsync(WalletFunding walletFunding);
        List<WalletFunding> FindWalletFundings(Expression<Func<WalletFunding, bool>> expression);
        Task<WalletFunding> GetWalletFundingByIdAsync(string id);
    }
}
