using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Interface
{
    public interface IKycRepository : IGenericRepository<Kyc>
    {
        Task<Kyc> GetKycByIdAsync(string id);
        List<Kyc> GetAllKycs();
        Task AddKycAsync(Kyc kyc);
        Task DeleteKycAsync(Kyc kyc);
        void UpdateKyc(Kyc kyc);
        Task<bool> FindKyc(Expression<Func<Kyc, bool>> expression);
    }
}
