using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Interface
{
    public interface ICardDetailsRepository : IGenericRepository<CardDetail>
    {
        List<CardDetail> GetCardDetailsAsync();
        Task AddCardDetailAsync(CardDetail cardDetail);
        Task DeleteCardDetailAsync(CardDetail cardDetail);
        Task DeleteAllCardDetailAsync(List<CardDetail> cardDetails);
        void UpdateCardDetailAsync(CardDetail cardDetail);
        List<CardDetail> FindCardDetails(Expression<Func<CardDetail, bool>> expression);
        Task<CardDetail> GetCardDetailByIdAsync(string id);
    }
}
