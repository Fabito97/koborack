using AutoMapper;
using Microsoft.AspNetCore.Http;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model;
using KoboRack.Model.Entities;
using KoboRack.Model.Enums;

namespace KoboRack.Core.Services
{
	public class WalletService : IWalletService
	{

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WalletService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


		public async Task<ApiResponse<CreditResponseDto>> FundWallet(FundWalletDto fundWalletDto)
		{
			try
			{
				var response = await GetWalletByNumber(fundWalletDto.WalletNumber);

				if (!response.Succeeded)
				{
					return ApiResponse<CreditResponseDto>.Failed(false, response.Message, response.StatusCode, response.Errors);
				}

				var wallet = response.Data;
				decimal bal = wallet.Balance + fundWalletDto.FundAmount;
				wallet.Balance = bal;
				_unitOfWork.WalletRepository.UpdateAsync(wallet);
				_unitOfWork.SaveChanges();

				var walletFunding = new WalletFunding
				{
					FundAmount = fundWalletDto.FundAmount,
					Narration = fundWalletDto.Naration,
					TransactionType = TransactionType.Credit,
					WalletId = wallet.Id

				};
				await _unitOfWork.WalletFundingRepository.AddAsync(walletFunding);
				_unitOfWork.SaveChanges();


				var creditResponse = new CreditResponseDto
				{
					WalletNumber = fundWalletDto.WalletNumber,
					Balance = bal,
					Message = "Your account has been credited successfully.",
				};

				return ApiResponse<CreditResponseDto>.Success(creditResponse, "Wallet funded successfully", StatusCodes.Status200OK);
			}
			catch (Exception e)
			{
				return ApiResponse<CreditResponseDto>.Failed(false, "Failed to fund wallet. ", StatusCodes.Status400BadRequest, new List<string>() { e.InnerException.ToString() });

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
			var wallets = _unitOfWork.WalletRepository.FindAsync(x => x.WalletNumber == phone);

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
