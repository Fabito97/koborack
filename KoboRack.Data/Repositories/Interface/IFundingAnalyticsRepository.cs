using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Interface
{
    public interface IFundingAnalyticsRepository : IGenericRepository<FundingAnalytics>
    {
        List<FundingAnalytics> GetFundingAnalyticssAsync();
        List<FundingAnalytics> GetFundingAnalytics(Expression<Func<FundingAnalytics, bool>> expression);
        Task<bool> CreateFundingAnalytics(FundingAnalytics fundingAnalytics);
        Task AddFundingAnalyticsAsync(FundingAnalytics fundingAnalytics);
        Task<FundingAnalytics> AddFundingAnalyticsAsync2(FundingAnalytics fundingAnalytics);
        Task DeleteFundingAnalyticsAsync(FundingAnalytics fundingAnalytics);
        Task DeleteAllFundingAnalyticsAsync(List<FundingAnalytics> fundingAnalyticss);
        void UpdateFundingAnalyticsAsync(FundingAnalytics fundingAnalytics);
        List<FundingAnalytics> FindFundingAnalyticss(Expression<Func<FundingAnalytics, bool>> expression);
        Task<FundingAnalytics> GetFundingAnalyticsByIdAsync(string id);
    }
}
