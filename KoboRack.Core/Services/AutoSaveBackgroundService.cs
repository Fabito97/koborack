using Microsoft.EntityFrameworkCore;
using KoboRack.Core.IServices;
using KoboRack.Data.Context;
using KoboRack.Model.Entities;
using KoboRack.Model.Enums;

namespace KoboRack.Core.Services
{
    public class AutoSaveBackgroundService : IAutoSaveBackgroundService
    {
        private readonly SaviDbContext _dbContext;
        private readonly IFundingService _fundingService;

        public AutoSaveBackgroundService(SaviDbContext dbContext, IFundingService fundingService)
        {
            _dbContext = dbContext;
            _fundingService = fundingService;
        }

        public async Task<bool> CheckAndExecuteAutoSaveTask()
        {
            var savings = await _dbContext.Savings.ToListAsync();
            if (savings.Count ==0)
            {
                return false;
            }
            var autoSavings = savings.Where(x=>x.AutoSave).ToList();
            if (autoSavings.Count == 0)
            {
                return false;
            }
            foreach (var saving in autoSavings)
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.AppUserId == saving.UserId);
                ArgumentNullException.ThrowIfNull(nameof(wallet));
                if (saving.AmountSaved >= saving.TargetAmount)
                {
                    throw new Exception("Target already achieved");
                }
                if (!saving.NextRuntime.HasValue)
                {
                    break;
                }
                DateTime nextRuntime = saving.NextRuntime.Value;
                DateTime currentDate = DateTime.Now.Date;

                int comparisonResult = nextRuntime.Date.CompareTo(currentDate);

                if (comparisonResult != 0)
                {
                    return false;
                }
                var savingGoalId = saving.Id;
                var amount = saving.AmountToAdd;
                var walletId = wallet.Id;
                var fundingServiceResult = await _fundingService.CreditPersonalTarget(walletId, savingGoalId, amount);
                if (!fundingServiceResult)
                {
                    throw new Exception("Auto-funding was not successful");
                }
                int multiplier;
                switch (saving.FundFrequency)
                {
                    case FundFrequency.Daily:
                        multiplier = 1;
                        break;
                    case FundFrequency.Weekly:
                        multiplier = 7;
                        break;
                    default:
                        multiplier = 31;
                        break;
                }
                saving.NextRuntime = DateTime.Now.AddDays(multiplier);
                _dbContext.Update(saving);
                await _dbContext.SaveChangesAsync();
            }
            return true;
        }

    }
}
