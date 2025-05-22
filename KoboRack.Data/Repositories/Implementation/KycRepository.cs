using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Implementation
{
    public class KycRepository : GenericRepository<Kyc>, IKycRepository
    {
        public KycRepository(SaviDbContext context) : base(context) {}

        public async Task AddKycAsync(Kyc kyc) => await AddAsync(kyc);

        public async Task DeleteKycAsync(Kyc kyc) => await DeleteAsync(kyc);

        public async Task<bool> FindKyc(Expression<Func<Kyc, bool>> expression) => await ExistsAsync(expression);

        public async Task<Kyc> GetKycByIdAsync(string id) => await GetByIdAsync(id);

        public List<Kyc> GetAllKycs() => GetAll();

        public void UpdateKyc(Kyc kyc) => UpdateAsync(kyc);
    }
}
