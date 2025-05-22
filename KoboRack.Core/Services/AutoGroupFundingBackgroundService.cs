using Microsoft.EntityFrameworkCore;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using KoboRack.Model.Enums;

namespace KoboRack.Core.Services
{
    public class AutoGroupFundingBackgroundService : IAutoGroupFundingBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupSavingsMembersRepository _groupSavingsMembersRepository;
        private readonly IWalletServices _walletServices;
        private readonly SaviDbContext _saviDbContext;

        public AutoGroupFundingBackgroundService(IUnitOfWork unitOfWork, 
            IGroupSavingsMembersRepository groupSavingsMembersRepository,
            IWalletServices walletServices,
            SaviDbContext saviDbContext)
        {
            _unitOfWork = unitOfWork;
            _groupSavingsMembersRepository = groupSavingsMembersRepository;
            _walletServices = walletServices;
            _saviDbContext = saviDbContext;
        }
        public async Task<bool> AutoGroup()
        {
            try
            {
                var allGroups = _unitOfWork.GroupRepository.GetAll();
                if (allGroups.Count == 0)
                    return false;
                var groups = allGroups.Where(group => group.GroupStatus == GroupStatus.OnGoing && group.NextRunTime.Date == DateTime.Today).ToList();
                if (groups.Count == 0)
                    return false;
                foreach (var group in groups)
                {
                    var amountToBeCredited = group.ContributionAmount * 4;
                    var postionToCredit = await _groupSavingsMembersRepository.GetNextPositionToPay(group.Id);
                    var memberToCredit = await _groupSavingsMembersRepository.FindAsync2(x => x.GroupSavingsId == group.Id && x.Positions == postionToCredit);
                    var MembersToDebit = _groupSavingsMembersRepository.FindAsync(x => x.GroupSavingsId == group.Id && x.Positions != postionToCredit);
                    foreach (var wallet in MembersToDebit)
                    {
                        await DebitUsersAsync(wallet.UserId, group.ContributionAmount, group.Id);
                    }
                    await CreditUserAsync(memberToCredit.UserId, amountToBeCredited, group.Id);
                    var totalPositionsInGroup = await _groupSavingsMembersRepository.GetTotalPositionsInGroup(group.Id);
                    var isLastMember = postionToCredit == totalPositionsInGroup;
                    if (!isLastMember)
                    {
                        group.NextRunTime = group.Schedule switch
                        {
                            FundFrequency.Daily => DateTime.Today,
                            FundFrequency.Weekly => DateTime.Now.AddDays(7),
                            _ => DateTime.Now.AddDays(31)
                        };
                         _unitOfWork.GroupRepository.UpdateAsync(group);
                    }
                    else
                    {
                        group.GroupStatus = GroupStatus.Ended;
                        _unitOfWork.GroupRepository.UpdateAsync(group);
                    }
                     _unitOfWork.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task CreditUserAsync(string userId, decimal amount, string groupId)
        {
            var userWallet = await _saviDbContext.Wallets.FirstOrDefaultAsync(w => w.AppUserId == userId);
            var memberToCredit = await _groupSavingsMembersRepository.FindAsync2(x => x.UserId == userId);
            ArgumentNullException.ThrowIfNull(nameof(userWallet));      
            userWallet.Balance += amount; 
             _unitOfWork.WalletRepository.UpdateAsync(userWallet);
            memberToCredit.IsPaid = true;
            _groupSavingsMembersRepository.UpdateGroupSavingsMember(memberToCredit);
            var groupTransaction = new GroupTransaction()
            {
                Amount = amount,
                TransactionType = TransactionType.Credit,
                AppUserId = userId,
                GroupId = groupId,
            };
            await _unitOfWork.GroupTransactionRepository.AddAsync(groupTransaction);
            _unitOfWork.SaveChanges();
        }
        private async Task DebitUsersAsync(string userId, decimal amount, string groupId)
        {
            var userWallet = await _saviDbContext.Wallets.FirstOrDefaultAsync(w => w.AppUserId == userId);
            ArgumentNullException.ThrowIfNull(nameof(userWallet));
            if (userWallet.Balance >= amount)
            {
                userWallet.Balance -= amount;
                _unitOfWork.WalletRepository.UpdateAsync(userWallet);
                var groupTransaction = new GroupTransaction()
                {
                    Amount = amount,
                    TransactionType = TransactionType.Debit,
                    AppUserId =userId,
                    GroupId=groupId,
                };
                await _unitOfWork.GroupTransactionRepository.AddAsync(groupTransaction);
                _unitOfWork.SaveChanges();
            }
            else
            {
                //This is the point where we populate the defaulting users table, for now we just throw an exception
                throw new InvalidOperationException("Insufficient balance.");
            }   
        }
    }
}
