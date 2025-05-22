namespace KoboRack.Core.IServices
{
    public interface IAutoSaveBackgroundService
    {
        public Task<bool> CheckAndExecuteAutoSaveTask();
    }
}
