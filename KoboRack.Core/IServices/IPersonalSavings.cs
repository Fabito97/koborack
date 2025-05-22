using KoboRack.Core.DTO;
using KoboRack.Model.Entities;

namespace KoboRack.Core.IServices
{
    public interface IPersonalSavings
    {
        Task<ResponseDto<string>> SetPersonal_Savings_Target(PersonalSavingsDTO saving, string userId);
        Task<ResponseDto<List<Saving>>> Get_ListOf_All_UserTargets(string UserId);
        Task<ResponseDto<Saving>> GetPersonalSavingsById(string personalSavingsId);
        Task<ResponseDto<decimal>> GetTotalGoalAmountByUser(string userId);
        public  Task<ResponseDto<int>> GetPersonalFundsCount();
    }
}
