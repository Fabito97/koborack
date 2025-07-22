using AutoMapper;
using Microsoft.AspNetCore.Http;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model;
using KoboRack.Model.Entities;
using KoboRack.Model.Enums;
using KoboRack.Core.DTO.PaystackDto;

namespace KoboRack.Core.Services
{
	public class WalletService : IWalletService
	{

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPaystackCustomerService _customerService;
        public WalletService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService, IPaystackCustomerService customerService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _customerService = customerService;
        }


        public async Task<ApiResponse<CreditResponseDto>> FundWalletAsync(FundWalletDto fundWalletDto)
        {
            var userId = _currentUserService.UserId;
            try
            {
                if (userId == null)
                    throw new UnauthorizedAccessException("User Id not found in token");

                var user = await _currentUserService.GetCurrentUserAsync();

                if (user == null)
                    return ApiResponse<CreditResponseDto>.Failed(false, "User not found", 401, null);

                var wallet = _unitOfWork.WalletRepository.Find(w => w.AppUserId == userId).FirstOrDefault();

                //var walletResponse = await GetWalletByNumber(fundWalletDto.WalletNumber);
                if (wallet == null)
                {
                    return ApiResponse<CreditResponseDto>.Failed(false, "User wallet not found", 404, null);
                }

                var reference = $"FUND_{Guid.NewGuid():N}".ToLower();

                // 2. Record Ledger Transaction
                var walletTransaction = new AppUserTransaction
                {
                    WalletId = wallet.Id,
                    AppUserId = user.Id,
                    Amount = fundWalletDto.FundAmount,
                    TransactionType = TransactionType.Credit,
                    Status = TransactionStatus.Pending,
                    Reference = reference,
                    Description = fundWalletDto.Naration ?? "Wallet top-up pending via card"
                    
                };
                await _unitOfWork.AppUserTransactionRepository.AddAsync(walletTransaction);
                _unitOfWork.SaveChanges();

                var paystackInit = await _customerService.InitiateWalletFundingAsync(user, walletTransaction, wallet.PaystackCustomerCode);

                var responseDto = new CreditResponseDto
                {
                    WalletNumber = wallet.WalletNumber,
                    Balance = wallet.Balance,
                    Reference = reference,
                    Message = "Payment initialized. Complete checkout to fund your wallet.",
                    PaymentUrl = paystackInit.AuthorizationUrl,
                    AccessCode = paystackInit.AccessCode
                };

                return ApiResponse<CreditResponseDto>.Success(responseDto, "Wallet funding initiated", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return ApiResponse<CreditResponseDto>.Failed(
                    false,
                    "Failed to initiate wallet funding.",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { error });
            }
        }

        public async Task<ApiResponse<List<WalletResponseDto>>> GetAllWallets()
		{
			var wallets = _unitOfWork.WalletRepository.GetAll();
			List<WalletResponseDto> result = new();
			foreach (var wallet in wallets)
			{
				var reponseDto = _mapper.Map<WalletResponseDto>(wallet);
				result.Add(reponseDto);
			}
			return new ApiResponse<List<WalletResponseDto>>(result, "Wallet retrieved successfully");
		}

		public async Task<ApiResponse<Wallet>> GetWalletByNumber(string phone)
		{
			var wallets = _unitOfWork.WalletRepository.Find(x => x.WalletNumber == phone);

			if (wallets.Count < 1)
			{
				return ApiResponse<Wallet>.Failed(false, "Wallet with this number not found", StatusCodes.Status404NotFound, new List<string>());
			}

			// Assuming you want to use the details of the first wallet if multiple wallets are found
			var firstWallet = wallets.First();

			return ApiResponse<Wallet>.Success(firstWallet, "Wallet retrieved successfully", StatusCodes.Status200OK);
		}
	}
}
