using KoboRack.Core.DTO;
using KoboRack.Model;

namespace KoboRack.Core.IServices
{
    public interface IAdminService
    {
        ApiResponse<GroupDTO> GetGroupSavingById (string groupId);
        Task<ApiResponse<GroupTransactionDto>> GetGroupTransactionsAsync(int page, int perPage);
    }
}
