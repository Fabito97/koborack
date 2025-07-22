using KoboRack.Core.DTO;
using KoboRack.Model;
using KoboRack.Model.Entities;

namespace KoboRack.Core.IServices
{
    public interface ITransactionService
    {
        Task<ApiResponse<List<TransactionDto>>> GetAllTransactionsAsync(string userId);
    }
}