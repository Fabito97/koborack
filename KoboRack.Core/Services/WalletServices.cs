using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Data.UnitOfWork;
using KoboRack.Model;
using KoboRack.Model.Entities;
using KoboRack.Utility;
using KoboRack.Core.Commons;
using KoboRack.Model.Enums;

namespace KoboRack.Core.Services
{
    public class WalletServices : IWalletServices
    {
        private readonly ILogger _logger;
        private readonly UserManager<AppUser> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IPaystackCustomerService _paystackService;
        private readonly ICurrentUserService _currentUserService;

        public WalletServices(ILogger<WalletServices> logger, UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IConfiguration configuration, IPaystackCustomerService paystackService, ICurrentUserService currentUserService)
        {
            _logger = logger;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            _httpClient = new HttpClient();
            _configuration = configuration;
            string secretKey = _configuration["PaystackApi:SecretKey"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");
            _paystackService = paystackService;
            _currentUserService = currentUserService;
        }


        public async Task<ApiResponse<string>> CreateWallet(AppUser user)
        {
            var userId = user.Id;
            var wallet = new Wallet();
            try
            {
                //var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found.", StatusCodes.Status400BadRequest, new List<string> { "User not found." });
                }

                var existingWallet = unitOfWork.WalletRepository.FindAsync(w => w.AppUserId == userId).FirstOrDefault();
                if (existingWallet != null && !string.IsNullOrEmpty(wallet?.PaystackCustomerCode))
                {
                    return ApiResponse<string>.Success(wallet.PaystackCustomerCode, "Paystack customer account already exist.", StatusCodes.Status200OK);                   
                }
               
                var setWallet = new SetWalletAccountNumber();
                wallet = existingWallet ?? new Wallet
                {
                    Id = Guid.NewGuid().ToString(),
                    WalletNumber = setWallet.SetWalletNumber(user.PhoneNumber),
                    Balance = 0,
                    AppUserId = user.Id,
                    TransactionPin = "1234"
                };

                if (existingWallet == null)
                    await unitOfWork.WalletRepository.AddWalletAsync(wallet);               

                var result = await _paystackService.CreateCustomerAsync(user, wallet.WalletNumber);

                if (!result.IsSuccess)
                {
                    throw new Exception($"Failed to create Paystack customer: {result.ErrorMessage}");
                }
               
                wallet.PaystackCustomerCode = result.CustomerCode;
                wallet.Reference = result.InternalId;
                unitOfWork.WalletRepository.UpdateAsync(wallet);
                unitOfWork.SaveChanges();            

                return ApiResponse<string>.Success(result.CustomerCode, "Wallet created successfully.", StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a wallet. " + ex.InnerException);
                return new ApiResponse<string>(false, "Error occurred while creating a wallet.", StatusCodes.Status500InternalServerError);
            }
        }


        public async Task<ApiResponse<string>> VerifyTransaction(string referenceCode)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(referenceCode))
            {
                return ApiResponse<string>.Failed(false, "Please provide a valid reference number", StatusCodes.Status400BadRequest, null);
            }

            try
            {
                var response = await _httpClient.GetAsync($"https://api.paystack.co/transaction/verify/{referenceCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var reason = $"Paystack error: {response.StatusCode} - {response.ReasonPhrase}";
                    return ApiResponse<string>.Failed(false, reason, StatusCodes.Status502BadGateway, null);
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<PaystackResponse>(content);

                if (result?.Status != true || result.Data == null)
                {
                    var reason = result?.Message ?? "Invalid response from Paystack";
                    return ApiResponse<string>.Failed(false, reason, StatusCodes.Status502BadGateway, null);
                }

                if (result.Data.Status != "success")
                {
                    return ApiResponse<string>.Failed(false, $"Payment was not successful. Status: {result.Data.Status}", StatusCodes.Status400BadRequest, null);
                }

                var amount = result.Data.Amount / 100;
                var email = result.Data.Customer?.Email ?? "unknown@email.com";

                var wallet = unitOfWork.WalletRepository.WalletById(userId);
                if (wallet == null)
                {
                    return ApiResponse<string>.Failed(false, "User wallet not found.", StatusCodes.Status404NotFound, null);
                }
                wallet.Balance += amount;
                wallet.ModifiedAt = DateTime.UtcNow;

                unitOfWork.WalletRepository.UpdateAsync(wallet);

                var transaction = unitOfWork.AppUserTransactionRepository
                    .FindAsync(w => w.Reference == referenceCode && w.Amount == amount)
                    .FirstOrDefault();

                if (transaction == null)
                {
                    return ApiResponse<string>.Failed(false, "Matching transaction not found.", StatusCodes.Status404NotFound, null);
                }

                transaction.Status = TransactionStatus.Success;

                //var walletResult = UpdateWallet(userId, amount);

                var walletFunding = new WalletFunding
                {
                    FundAmount = amount,
                    Reference = referenceCode,
                    WalletNumber = wallet.WalletNumber,
                    WalletId = wallet.Id,
                    CumulativeAmount = wallet.Balance,
                    TransactionType = Model.Enums.TransactionType.Credit
                };

                await unitOfWork.WalletFundingRepository.AddAsync(walletFunding);
                unitOfWork.SaveChanges(); // Consider using async

                var successMessage = $"Payment of {amount:N0} Naira from {email} was successful!";
                return ApiResponse<string>.Success(successMessage, successMessage, StatusCodes.Status200OK);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while verifying Paystack transaction");
                return ApiResponse<string>.Failed(false, "Unable to reach Paystack", StatusCodes.Status503ServiceUnavailable, new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error verifying Paystack transaction");
                return ApiResponse<string>.Failed(false, "Transaction verification failed", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
        }

        public ResponseDto<WalletDto> GetUserWalletAsync(string userId)
        {
            var wallet = unitOfWork.WalletRepository.WalletById(userId);

            if (wallet == null)
            {
                return new ResponseDto<WalletDto>()
                {
                    StatusCode = 404,
                    DisplayMessage = "User does not have a wallet.",
                };
            }
            var walletDto = new WalletDto
            {
                Currency = wallet.Currency,
                AppUserId = wallet.AppUserId,
                Reference = wallet.Reference,
                Balance = wallet.Balance,
                WalletNumber = wallet.WalletNumber,
                Id = wallet.Id,
            };
            return new ResponseDto<WalletDto>()
            {
                StatusCode = 200,
                DisplayMessage = "User's wallet retrieved successfully.",
                Result = walletDto,
            };
        }
        private ResponseDto<Wallet> UpdateWallet(string userId, decimal amount)
        {
            var wallet = unitOfWork.WalletRepository.WalletById(userId);
            if (wallet == null)
            {
                return new ResponseDto<Wallet>
                {
                    StatusCode = 400,
                    DisplayMessage = "User does not have a wallet",
                };
            }
            wallet.Balance += amount;
            wallet.ModifiedAt = DateTime.UtcNow;

            unitOfWork.WalletRepository.UpdateAsync(wallet);
            unitOfWork.SaveChanges();
            return new ResponseDto<Wallet>
            {
                StatusCode = 200,
                DisplayMessage = "wallet updated successfully",
                Result = wallet
            };
        }

        public async Task<ResponseDto<decimal>> GetTotalCustomerWalletBalanceAsync()
        {
            try
            {
                var totalBalance = unitOfWork.WalletRepository.GetAll().Sum(w => w.Balance);

                return new ResponseDto<decimal>
                {
                    StatusCode = 200,
                    DisplayMessage = "Total wallet balance retrieved successfully.",
                    Result = totalBalance
                };
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Error occurred while retrieving total wallet balance.");

                
                return new ResponseDto<decimal>
                {
                    StatusCode = 500,
                    DisplayMessage = "An error occurred while retrieving total wallet balance.",
                    Result = 0 
                };
            }

        }
    }
}

