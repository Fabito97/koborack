using KoboRack.Core.DTO;
using KoboRack.Model;
using KoboRack.Model.Entities;

namespace KoboRack.Core.IServices
{
	public  interface IWalletService
	{
		//Task<ApiResponse<bool>> CreateWallet(CreateWalletDto createWalletDto);
		Task<ApiResponse<List<WalletResponseDto>>> GetAllWallets();
		Task<ApiResponse<Wallet>> GetWalletByNumber(string phone);
		Task<ApiResponse<CreditResponseDto>> FundWallet(FundWalletDto fundWalletDto);        
    }
}
