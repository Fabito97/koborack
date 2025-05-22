using AutoMapper;
using Microsoft.EntityFrameworkCore;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using KoboRack.Model.Enums;

namespace KoboRack.Core.Services
{
    public class PersonalSavings : IPersonalSavings
    {
        private readonly ISavingRepository _savingRepository;
        private readonly ICloudinaryServices<Saving> _cloudinaryServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PersonalSavings(ISavingRepository savingRepository, ICloudinaryServices<Saving> cloudinaryServices, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _savingRepository = savingRepository;
            _cloudinaryServices = cloudinaryServices;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<string>> SetPersonal_Savings_Target(PersonalSavingsDTO saving, string userId)
        {
            var response = new ResponseDto<string>();

            try
            {
                saving.NextRuntime = DateTime.Today;

                if (saving.TargetAmount <= 0 || saving.AmountToAdd <= 0)
                {
                    SetErrorResponse(response, "TargetAmount or AmountToAdd cannot be less than or equal to zero", 400);
                    return response;
                }

                decimal multiplier;
                switch (saving.FundFrequency)
                {
                    case FundFrequency.Daily:
                        multiplier = 1;
                        break;
                    case FundFrequency.Weekly:
                        multiplier = 7;
                        break;
                    default:
                        multiplier = 31;
                        break;
                }

                var t = (saving.TargetAmount / saving.AmountToAdd) * multiplier;
                double z = (double)t;
                saving.EndDate = DateTime.Now.AddDays(z);
                saving.WithdrawalDate = saving.EndDate.AddDays(1);
                saving.UserId = userId;
                var personalSaving = _mapper.Map<Saving>(saving);

                var newTarget = await _savingRepository.CreateSavings(personalSaving);
                var goalUrl = await _cloudinaryServices.UploadImage(personalSaving.Id, saving.GoalUrl);
                personalSaving.GoalUrl = goalUrl;
                _savingRepository.UpdateAsync(personalSaving);
                await _savingRepository.SaveChangesAsync();
                if (newTarget)
                {
                    SetSuccessResponse(response, $"Your target of amount {personalSaving.TargetAmount} has been successfully created", 200);
                }
                else
                {
                    SetErrorResponse(response, $"Unable to create target of amount {personalSaving.TargetAmount}", 400);
                }

                return response;
            }
            catch (Exception ex)
            {
                SetErrorResponse(response, ex.Message, 400);
                return response;
            }
        }
        private static void SetErrorResponse(ResponseDto<string> response, string errorMessage, int statusCode)
        {
            response.DisplayMessage = "Failed";
            response.Result = errorMessage;
            response.StatusCode = statusCode;
        }
        private static void SetSuccessResponse(ResponseDto<string> response, string successMessage, int statusCode)
        {
            response.DisplayMessage = "Success";
            response.Result = successMessage;
            response.StatusCode = statusCode;
        }
        private static void SetSuccessResponse<T>(ResponseDto<T> response, string successMessage, T result, int statusCode)
        {
            response.DisplayMessage = "Success";
            response.Result = result;
            response.StatusCode = statusCode;
        }
        private static void SetNotFoundResponse<T>(ResponseDto<T> response, string errorMessage, int statusCode)
        {
            response.DisplayMessage = "Failed";
            response.Result = default(T); // or null, depending on the type T
            response.StatusCode = statusCode;
        }
        private static void SetErrorResponse<T>(ResponseDto<T> response, string errorMessage, int statusCode)
        {
            response.DisplayMessage = "Failed";
            response.Result = default(T);
            response.StatusCode = statusCode;
        }
        public async Task<ResponseDto<List<Saving>>> Get_ListOf_All_UserTargets(string userId)
        {
            var response = new ResponseDto<List<Saving>>();

            try
            {
                var listOfTargets = await _savingRepository.GetAllSetSavingsByUserId(userId);

                if (listOfTargets.Any())
                {
                    SetSuccessResponse(response, "Targets retrieved successfully", listOfTargets, 200);
                }
                else
                {
                    SetNotFoundResponse(response, "No targets found for the user", 404);
                }
            }
            catch (Exception ex)
            {
                SetErrorResponse(response, ex.Message, 500);
            }

            return response;
        }

        public async Task<ResponseDto<Saving>> GetPersonalSavingsById(string personalSavingsId)
        {
            var response = new ResponseDto<Saving>();

            try
            {
                var personalSavings = await _savingRepository.GetSavingByIdAsync(personalSavingsId);

                if (personalSavings != null)
                {
                    SetSuccessResponse(response, "Personal savings details retrieved successfully", personalSavings, 200);
                }
                else
                {
                    SetNotFoundResponse(response, "Personal savings details not found", 404);
                }
            }
            catch (Exception ex)
            {
                SetErrorResponse(response, ex.Message, 500);
            }

            return response;
        }

        public async Task<ResponseDto<decimal>> GetTotalGoalAmountByUser(string userId)
        {
            var response = new ResponseDto<decimal>();

            try
            {
                var listOfTargets = await _savingRepository.GetAllSetSavingsByUserId(userId);

                if (listOfTargets.Any())
                {
                    var totalGoalAmount = listOfTargets.Sum(target => target.AmountSaved);
                    SetSuccessResponse(response, "Total goal amount retrieved successfully", totalGoalAmount, 200);
                }
                else
                {
                    SetNotFoundResponse(response, "No targets found for the user", 404);
                }
            }
            catch (Exception ex)
            {
                SetErrorResponse(response, ex.Message, 500);
            }

            return response;
        }


        public async Task<ResponseDto<int>> GetPersonalFundsCount()
        {
            var response = new ResponseDto<int>();
            int count = 0;
            try
            {
                var result =  _unitOfWork.WalletFundingRepository.GetAll();
                foreach (var item in result) 
                {
                    if (item.TransactionType == TransactionType.Debit)
                        count += 1;
                }
                //if (count <= 0)
                //{
                //    return new ResponseDto<int>()
                //    {
                //        DisplayMessage = "0 personal group credited so far",
                //        Result = 0,
                //        StatusCode = 200
                //    };
                //}
                return new ResponseDto<int>()
                {
                    DisplayMessage = $"{count} personal group credited so far",
                    Result = count,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {              
                SetErrorResponse(response, ex.Message, 500);
            }
            return response;
        }
    }
 }


