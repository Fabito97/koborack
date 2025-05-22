using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Interface
{
    public interface IWalletRepository : IGenericRepository<Wallet>
    {
        List<Wallet> GetWalletsAsync();
        Task AddWalletAsync(Wallet wallet);
        Task DeleteWalletAsync(Wallet wallet);
        Task DeleteAllWalletAsync(List<Wallet> wallets);
        void UpdateWalletAsync(Wallet wallet);
        List<Wallet> FindWallets(Expression<Func<Wallet, bool>> expression);
        Task<Wallet> GetWalletByIdAsync(string id);
        Wallet WalletById(string userId);
    }
}
