using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using KoboRack.Model.Enums;

namespace KoboRack.Core.Services
{
    public class FundingAnalyticsBackgroundServices : IFundingAnalyticsBackgroundServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPersonalSavings _personalSavings;

        public FundingAnalyticsBackgroundServices(IUnitOfWork unitOfWork, IPersonalSavings personalSavings)
        {
            _unitOfWork = unitOfWork;
            _personalSavings = personalSavings;
        }

        public async Task<ResponseDto<string>> SWCFunding()
        {
            try
            {
                var funding = _unitOfWork.WalletFundingRepository.GetAll();
                var savings = CalculateTotalFunds(funding, referenceNotNull: true, createdAt: DateTime.UtcNow.Date.AddDays(-1));
                var withdrawals = CalculateTotalFunds(funding, referenceNotNull: false, createdAt: DateTime.UtcNow.Date.AddDays(-1));

                decimal CalculateTotalFunds(List<WalletFunding> fundingQuery, bool referenceNotNull, DateTime createdAt)
                {
                    return fundingQuery
                        .Where(x => (x.Reference != null) == referenceNotNull && x.CreatedAt.Date == createdAt)
                        .Sum(x => x.FundAmount);
                }
                var contributionsList = funding.Where(x => x.TransactionType == TransactionType.Debit && x.CreatedAt == DateTime.UtcNow.Date.AddDays(-1)).ToList();
                var contributions = contributionsList.Sum(x => x.FundAmount);
                var fundinAnalytics = new FundingAnalytics()
                {
                    Savings = savings,
                    Withrawal = withdrawals,
                    Contribution = contributions
                };
                var result = await _unitOfWork.FundingAnalyticsRepository.CreateFundingAnalytics(fundinAnalytics);
                if (!result)
                {
                    return new ResponseDto<string>()
                    {
                        DisplayMessage = "Failed to create analytics",
                        StatusCode = 201,
                        Result = null
                    };
                }
                return new ResponseDto<string>()
                {
                    DisplayMessage = "Table populated successfully",
                    StatusCode = 200,
                    Result = null
                };
            }
            catch (Exception ex)
            {

                return new ResponseDto<string>()
                {
                    DisplayMessage = ex.Message,
                    StatusCode = 500,
                    Result = null
                };
            }
        }
        
    }
}
