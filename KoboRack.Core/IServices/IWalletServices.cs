using KoboRack.Core.DTO;
using KoboRack.Model;

namespace KoboRack.Core.IServices
{
    public interface IWalletServices
    {
        Task<ApiResponse<string>> CreateWallet(string userId);
       ResponseDto<WalletDto> GetUserWalletAsync(string userId);
        Task<string> VerifyTransaction(string referenceCode, string userId);
        Task<ResponseDto<decimal>> GetTotalCustomerWalletBalanceAsync();
    }
}
