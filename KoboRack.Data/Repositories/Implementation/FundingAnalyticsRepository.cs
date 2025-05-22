using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Implementation
{
    public class FundingAnalyticsRepository : GenericRepository<FundingAnalytics>, IFundingAnalyticsRepository
    {
        public FundingAnalyticsRepository(SaviDbContext context) : base(context)
        {
        }
        public async Task AddFundingAnalyticsAsync(FundingAnalytics fundingAnalytics)
        {
            await AddAsync(fundingAnalytics);
        }

        public async Task<FundingAnalytics> AddFundingAnalyticsAsync2(FundingAnalytics fundingAnalytics)
        {
            return AddAsync2(fundingAnalytics);
        }

        public async Task<bool> CreateFundingAnalytics(FundingAnalytics fundingAnalytics)
        {
           var funding = await CreateAsync(fundingAnalytics);
            if (!funding)
            {
                return false;
            }
            return true;
        }

        public async Task DeleteAllFundingAnalyticsAsync(List<FundingAnalytics> fundingAnalyticss)
        {
            await DeleteAllAsync(fundingAnalyticss);
        }

        public async Task DeleteFundingAnalyticsAsync(FundingAnalytics fundingAnalytics)
        {
            await DeleteAsync(fundingAnalytics);
        }

        public List<FundingAnalytics> FindFundingAnalyticss(Expression<Func<FundingAnalytics, bool>> expression)
        {
            return FindAsync(expression);
        }

        public List<FundingAnalytics> GetFundingAnalytics(Expression<Func<FundingAnalytics, bool>> expression)
        {
            return GetAll(expression);
        }

        public async Task<FundingAnalytics> GetFundingAnalyticsByIdAsync(string id)
        {
            return await GetByIdAsync(id);
        }

        public List<FundingAnalytics> GetFundingAnalyticssAsync()
        {
            return GetAll();
        }

        public void UpdateFundingAnalyticsAsync(FundingAnalytics fundingAnalytics)
        {
            UpdateAsync(fundingAnalytics);
        }
    }
}
