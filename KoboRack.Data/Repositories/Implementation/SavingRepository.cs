using Microsoft.EntityFrameworkCore;
using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Implementation
{
    public class SavingRepository : GenericRepository<Saving>, ISavingRepository
    {
        private readonly SaviDbContext _context;
        public SavingRepository(SaviDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddSavingAsync(Saving saving)
        {
           await AddAsync(saving);
        }
        public async Task<bool> CreateSavings(Saving saving)
        {
            var savings =  await CreateAsync(saving);
            if (!savings)
            {
                return false;
            }
            return true;
        }

        public async Task DeleteAllSavingAsync(List<Saving> savings)
        {
           await DeleteAllAsync(savings);
        }

        public async Task DeleteSavingAsync(Saving saving)
        {
            await DeleteAsync(saving);
        }

        public List<Saving> FindSavings(Expression<Func<Saving, bool>> expression)
        {
            return FindAsync(expression);
        }
        public async Task<Saving> GetSavingByIdAsync(string id)
        {
            return await GetByIdAsync(id);
        }

        public List<Saving> GetSavingsAsync()
        {
            return GetAll();
        }

        public async Task<List<Saving>> GetAllSetSavingsByUserId(string userId)
        {          
            return await _context.Savings
            .Where(t => t.UserId == userId)
            .ToListAsync();            
        }

        public void UpdateSavingAsync(Saving saving)
        {
            UpdateAsync(saving);
        }
    }
}
