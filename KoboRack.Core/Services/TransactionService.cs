using KoboRack.Core.IServices;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model;
using KoboRack.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAppUserTransactionRepository _userTransaction;
        private readonly UserManager<AppUser> _userManager;

        public TransactionService(IAppUserTransactionRepository userTransaction, UserManager<AppUser> userManager)
        {
            _userTransaction = userTransaction;
            _userManager = userManager;
        }

        public async Task<ApiResponse<List<AppUserTransaction>>> GetAllTransactionsAsync(string userId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse<List<AppUserTransaction>>(false, "User not found.", StatusCodes.Status404NotFound);
            }

            var result = _userTransaction.FindAppUserTransactions(w => w.AppUserId == userId).ToList();

            if (result.Count == 0)
            {
                new ApiResponse<List<AppUserTransaction>>(true, "No transactions available", StatusCodes.Status200OK, result);
            }

            return new ApiResponse<List<AppUserTransaction>>(true, "Transactions retrieved successfully", StatusCodes.Status200OK, result);
        }
    }
}
