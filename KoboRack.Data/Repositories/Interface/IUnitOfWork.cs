namespace KoboRack.Data.Repositories.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IAppUserTransactionRepository AppUserTransactionRepository { get; }
        ICardDetailsRepository CardDetailsRepository { get; }
        IGroupRepository GroupRepository { get; }
        IGroupTransactionRepository GroupTransactionRepository { get; }
        IKycRepository KycRepository { get; }
        IOtpRepository OtpRepository { get; }
        ISavingRepository SavingRepository { get; }
        IWalletFundingRepository WalletFundingRepository { get; }
        IWalletRepository WalletRepository { get; }
        IFundingAnalyticsRepository FundingAnalyticsRepository { get; }
        int SaveChanges();
    }
}
