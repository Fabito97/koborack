using KoboRack.Core.DTO;
using KoboRack.Model;
using KoboRack.Model.Entities;

namespace KoboRack.Core.IServices
{
    public interface IWalletServices
    {
        Task<ApiResponse<string>> CreateWallet(AppUser userId);
       ResponseDto<WalletDto> GetUserWalletAsync(string userId);
        Task<ApiResponse<string>> VerifyTransaction(string referenceCode);
        Task<ResponseDto<decimal>> GetTotalCustomerWalletBalanceAsync();
    }
}
