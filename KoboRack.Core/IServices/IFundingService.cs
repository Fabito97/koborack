namespace KoboRack.Core.IServices
{
    public interface IFundingService
    {
        public Task<bool> CreditSavingsGoal(string savingsGoalId, decimal amount);
        public Task<bool> DebitWallet(string walletId, decimal amount);
        public Task<bool> CreditPersonalTarget(string walletId, string savingsGoalId, decimal amount);
        public Task<bool> DebitPersonalTarget(string walletId, string savingsGoalId, decimal amount);
        public Task<bool> CreditWallet(string walletId, decimal amount);
        public Task<bool> DebitSavingsGoal(string savingsGoalId, decimal amount);
        
    }
}
