using KoboRack.Core.DTO;

namespace KoboRack.Core.IServices
{
    public interface IFundingAnalyticsBackgroundServices
    {
        public Task<ResponseDto<string>> SWCFunding();
    }
}
