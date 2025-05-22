namespace KoboRack.Core.IServices
{
    public interface IAutoGroupFundingBackgroundService
    {
        Task<bool> AutoGroup();
    }
}
