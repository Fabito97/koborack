using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Interface
{
    public interface ISavingRepository : IGenericRepository<Saving>
    {
        List<Saving> GetSavingsAsync();
        Task AddSavingAsync(Saving saving);
        Task<bool> CreateSavings(Saving saving);
        Task DeleteSavingAsync(Saving saving);
        Task DeleteAllSavingAsync(List<Saving> savings);
        void UpdateSavingAsync(Saving saving);
        List<Saving> FindSavings(Expression<Func<Saving, bool>> expression);
        Task<Saving> GetSavingByIdAsync(string id);
        Task<List<Saving>> GetAllSetSavingsByUserId(string userId);
    }
}
