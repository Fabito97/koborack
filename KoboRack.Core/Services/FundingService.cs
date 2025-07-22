using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KoboRack.Core.IServices;
using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Implementation;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Data.UnitOfWork;
using KoboRack.Model.Entities;
using KoboRack.Model.Enums;
namespace KoboRack.Core.Services
{
    public class FundingService : IFundingService
    {
        private readonly SaviDbContext _dbContext;
        private readonly ILogger<FundingService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public FundingService(SaviDbContext dbContext, ILogger<FundingService> logger, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreditPersonalTarget(string walletId, string savingsGoalId, decimal amount)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                if (!await DebitWallet(walletId, amount))
                {
                    _logger.LogWarning("Failed to debit wallet {WalletId} for amount {Amount}", walletId, amount);
                    await transaction.RollbackAsync();
                    return false;
                }

                if (!await CreditSavingsGoal(savingsGoalId, amount))
                {
                    _logger.LogWarning("Failed to credit savings goal {GoalId} for amount {Amount}", savingsGoalId, amount);
                    await transaction.RollbackAsync();
                    return false;
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred during fund transfer from wallet {WalletId} to goal {GoalId}", walletId, savingsGoalId);
                throw;
            }
        }

        public async Task<bool> DebitPersonalTarget(string walletId, string savingsGoalId, decimal amount)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                if (!await DebitSavingsGoal(savingsGoalId, amount))
                {
                    _logger.LogWarning("Failed to debit savings goal {GoalId} for amount {Amount}", savingsGoalId, amount);
                    await transaction.RollbackAsync();
                    return false;
                }

                if (!await CreditWallet(walletId, amount))
                {
                    _logger.LogWarning("Failed to credit wallet {WalletId} for amount {Amount}", walletId, amount);
                    await transaction.RollbackAsync();
                    return false;
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred during fund reverse from goal {GoalId} to wallet {WalletId}", savingsGoalId, walletId);
                throw;
            }
        }

        public async Task<bool> CreditSavingsGoal(string savingsGoalId, decimal amount)
        {
            try
            {
                var goal = await _unitOfWork.SavingRepository.GetSavingByIdAsync(savingsGoalId);
                if (goal == null) return false;

                goal.AmountSaved += amount;
                _unitOfWork.SavingRepository.UpdateSavingAsync(goal);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crediting savings goal {GoalId}", savingsGoalId);
                throw;
            }
        }

        public async Task<bool> DebitSavingsGoal(string savingsGoalId, decimal amount)
        {
            try
            {
                var goal = await _unitOfWork.SavingRepository.GetSavingByIdAsync(savingsGoalId);
                if (goal == null || goal.AmountSaved < amount || amount <= 0) return false;

                goal.AmountSaved -= amount;
                _unitOfWork.SavingRepository.UpdateSavingAsync(goal);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error debiting savings goal {GoalId}", savingsGoalId);
                throw;
            }
        }

        public async Task<bool> CreditWallet(string walletId, decimal amount)
        {
            try
            {
                var wallet = await _unitOfWork.WalletRepository.GetWalletByIdAsync(walletId);
                if (wallet == null) return false;

                wallet.Balance += amount;
                wallet.LockedAmount -= amount; // optionally remove if computing dynamically

                _unitOfWork.WalletRepository.UpdateWalletAsync(wallet);

                await _unitOfWork.WalletFundingRepository.AddAsync(new WalletFunding
                {
                    FundAmount = amount,
                    WalletNumber = wallet.WalletNumber,
                    WalletId = wallet.Id,
                    Narration = "Credited from personal goal",
                    CumulativeAmount = wallet.Balance,
                    TransactionType = TransactionType.Credit
                });

                _unitOfWork.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crediting wallet {WalletId}", walletId);
                throw;
            }
        }

        public async Task<bool> DebitWallet(string walletId, decimal amount)
        {
            try
            {
                var wallet = await _unitOfWork.WalletRepository.GetWalletByIdAsync(walletId);
                if (wallet == null || wallet.Balance < amount || amount <= 0) return false;

                wallet.Balance -= amount;
                wallet.LockedAmount += amount; // optionally remove if computing dynamically

                _unitOfWork.WalletRepository.UpdateWalletAsync(wallet);

                await _unitOfWork.WalletFundingRepository.AddAsync(new WalletFunding
                {
                    FundAmount = amount,
                    WalletNumber = wallet.WalletNumber,
                    WalletId = wallet.Id,
                    Narration = "Savings for personal goal",
                    CumulativeAmount = wallet.Balance,
                    TransactionType = TransactionType.Debit
                });

                _unitOfWork.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error debiting wallet {WalletId}", walletId);
                throw;
            }
        }
    }


}
