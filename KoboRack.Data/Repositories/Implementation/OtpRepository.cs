using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Implementation
{
    public class OtpRepository : GenericRepository<Otp>, IOtpRepository
    {
        public OtpRepository(SaviDbContext context) : base(context)
        {
        }

        public async Task AddOtpAsync(Otp otp)
        {
            await AddAsync(otp);
        }

        public async Task DeleteAllOtpAsync(List<Otp> otps)
        {
           await DeleteAllAsync(otps);
        }

        public async Task DeleteOtpAsync(Otp otp)
        {
           await DeleteAsync(otp);
        }

        public List<Otp> FindOtps(Expression<Func<Otp, bool>> expression)
        {
            return FindAsync(expression).ToList();
        }

        public async Task<Otp> GetOtpByIdAsync(string id)
        {
            return await GetByIdAsync(id);
        }

        public List<Otp> GetOtpsAsync()
        {
            return GetAll();
        }

        public void UpdateOtpAsync(Otp otp)
        {
            UpdateAsync(otp);
        }
    }
}
