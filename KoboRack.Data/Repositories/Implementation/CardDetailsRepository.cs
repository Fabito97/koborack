using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Implementation
{
    public class CardDetailsRepository : GenericRepository<CardDetail>, ICardDetailsRepository
    {
        public CardDetailsRepository(SaviDbContext context) : base(context)
        {
        }

        public async Task AddCardDetailAsync(CardDetail cardDetail)
        {
            await AddAsync(cardDetail);
        }

        public async Task DeleteAllCardDetailAsync(List<CardDetail> cardDetails)
        {
           await DeleteAllAsync(cardDetails);
        }

        public async Task DeleteCardDetailAsync(CardDetail cardDetail)
        {
            await DeleteAsync(cardDetail);
        }

        public List<CardDetail> FindCardDetails(Expression<Func<CardDetail, bool>> expression)
        {
            return FindAsync(expression);
        }
        public async Task<CardDetail> GetCardDetailByIdAsync(string id)
        {
            return  await GetByIdAsync(id);
        }

        public List<CardDetail> GetCardDetailsAsync()
        {
            return GetAll();
        }

        public void UpdateCardDetailAsync(CardDetail cardDetail)
        {
            UpdateAsync(cardDetail);
        }
    }
}
