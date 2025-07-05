using KoboRack.Core.Commons;
using KoboRack.Core.DTO.PaystackDto;
using KoboRack.Model.Entities;

namespace KoboRack.Core.IServices
{
    public interface IPaystackCustomerService
    {
        Task<PaystackCustomerResult> CreateCustomerAsync(AppUser user, string walletNumber);
        Task<PaystackInitResponseDto> InitiateWalletFundingAsync(AppUser user, AppUserTransaction walletTransaction, string customerCode);
    }
}