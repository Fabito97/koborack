using KoboRack.Core.DTO;
using KoboRack.Model.Entities;

namespace KoboRack.Core.IServices
{
    public interface IPersonalSavings
    {
        Task<ResponseDto<string>> SetUserGoal(PersonalSavingsDTO saving);
        Task<ResponseDto<List<GetUserSavingsDto>>> GetUserGoals(string userId);
        Task<ResponseDto<Saving>> GetPersonalSavingsById(string personalSavingsId);
        Task<ResponseDto<decimal>> GetUserTotalGoalAmount(string userId);
        public  Task<ResponseDto<int>> GetPersonalFundsCount();
        Task<ResponseDto<Saving>> UpdatePersonalSavings(string personalSavingsId, PersonalSavingsDTO savings);
        Task<ResponseDto<string>> DeletePersonalSavings(string personalSavingsId);
    }
}
