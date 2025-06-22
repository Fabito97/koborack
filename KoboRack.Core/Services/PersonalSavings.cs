using AutoMapper;
using Microsoft.EntityFrameworkCore;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using KoboRack.Model.Enums;
using Microsoft.AspNetCore.Identity;

namespace KoboRack.Core.Services
{
    public class PersonalSavings : IPersonalSavings
    {
        private readonly ISavingRepository _savingRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICloudinaryServices<Saving> _cloudinaryServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PersonalSavings(ISavingRepository savingRepository, UserManager<AppUser> userManager, ICloudinaryServices<Saving> cloudinaryServices, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _savingRepository = savingRepository;
            _cloudinaryServices = cloudinaryServices;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<ResponseDto<string>> SetPersonal_Savings_Target(PersonalSavingsDTO saving, string userId)
        {
            var response = new ResponseDto<string>();

            try
            {
                // Validate amounts
                if (saving.TargetAmount <= 0 || saving.AmountToAdd <= 0)
                {
                    SetErrorResponse(response, "TargetAmount and AmountToAdd must be greater than zero.", 400);
                    return response;
                }

                // Calculate number of savings cycles required
                int numPayments = (int)Math.Ceiling(saving.TargetAmount / saving.AmountToAdd);

                // Start from today
                DateTime currentDate = DateTime.Today;

                // Calculate EndDate
                for (int i = 0; i < numPayments; i++)
                {
                    switch (saving.FundFrequency)
                    {
                        case FundFrequency.Daily:
                            currentDate = currentDate.AddDays(1);
                            break;

                        case FundFrequency.Weekly:
                            currentDate = currentDate.AddDays(7);
                            break;

                        case FundFrequency.Monthly:
                            currentDate = currentDate.AddMonths(1);
                            break;

                        default:
                            SetErrorResponse(response, "Unsupported FundFrequency.", 400);
                            return response;
                    }
                }

                var newGoal = new Saving();

                newGoal.EndDate = currentDate;
                newGoal.WithdrawalDate = newGoal.EndDate.AddDays(1);

                // Set NextRuntime based on AutoSave
                if (saving.AutoSave)
                {
                    // Schedule next payment according to FundFrequency
                    DateTime nextRuntime = DateTime.Today;
                    switch (saving.FundFrequency)
                    {
                        case FundFrequency.Daily:
                            nextRuntime = nextRuntime.AddDays(1);
                            break;

                        case FundFrequency.Weekly:
                            nextRuntime = nextRuntime.AddDays(7);
                            break;

                        case FundFrequency.Monthly:
                            nextRuntime = nextRuntime.AddMonths(1);
                            break;
                    }
                    newGoal.NextRuntime = nextRuntime;
                }
                else
                {
                    // If AutoSave is OFF → NextRuntime is null (manual)
                    newGoal.NextRuntime = null;
                }

                newGoal.UserId = userId;
                newGoal.Description = saving.Description;
                newGoal.TargetAmount = saving.TargetAmount;
                newGoal.TargetName = saving.TargetName;
                newGoal.AmountToAdd = saving.AmountToAdd;
                newGoal.FundFrequency = saving.FundFrequency;
                newGoal.AutoSave = saving.AutoSave;

                //var personalSaving = _mapper.Map<Saving>(saving);

                // Save to database
                var newTarget = await _savingRepository.CreateSavings(newGoal);

                // Upload goal image (if provided)
                if (saving.GoalUrl != null)
                {
                    var goalUrl = await _cloudinaryServices.UploadImage(newGoal.Id, saving.GoalUrl);

                    newGoal.GoalUrl = goalUrl;
                }

                // Update record with goal URL
                _savingRepository.UpdateAsync(newGoal);
                await _savingRepository.SaveChangesAsync();

                // Return response
                if (newTarget)
                {
                    SetSuccessResponse(response, $"Your savings target of ₦{newGoal.TargetAmount} has been created successfully.", 200);
                }
                else
                {
                    SetErrorResponse(response, "Unable to create savings target.", 400);
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
        public async Task<ResponseDto<List<GetUserSavingsDto>>> Get_ListOf_All_UserTargets(string userId)
        {
            var response = new ResponseDto<List<GetUserSavingsDto>>();

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    SetNotFoundResponse(response, "User not found", 404);
                }
                var listOfTargets = await _savingRepository.GetAllSetSavingsByUserId(userId);

                var savings = _mapper.Map<List<GetUserSavingsDto>>(listOfTargets);

                SetSuccessResponse(response, "Targets retrieved successfully", savings, 200);

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

        public async Task<ResponseDto<Saving>> UpdatePersonalSavings(string personalSavingsId, PersonalSavingsDTO savings)
        {
            var response = new ResponseDto<Saving>();

            try
            {
                var user = await _userManager.FindByEmailAsync(savings.UserId);

                if (user == null)
                {
                    SetErrorResponse(response, "User not found", 404);
                    return response;
                }

                var personalSavings = await _savingRepository.GetSavingByIdAsync(personalSavingsId);

                if (personalSavings == null)
                {
                    SetNotFoundResponse(response, "Personal savings details not found", 404);
                    return response;
                }

                // Update fields
                personalSavings.AmountToAdd = savings.AmountToAdd;
                personalSavings.TargetName = savings.TargetName;
                personalSavings.Description = savings.Description;
                personalSavings.FundFrequency = savings.FundFrequency;
                personalSavings.TargetAmount = savings.TargetAmount;
                personalSavings.AutoSave = savings.AutoSave;

                if (savings.GoalUrl != null)
                {
                    personalSavings.GoalUrl = await _cloudinaryServices.UploadImage(personalSavingsId, savings.GoalUrl);
                }

                // Recalculate EndDate, WithdrawalDate
                decimal cyclesNeeded = personalSavings.TargetAmount / personalSavings.AmountToAdd;

                DateTime endDate;

                switch (personalSavings.FundFrequency)
                {
                    case FundFrequency.Daily:
                        endDate = DateTime.Now.AddDays((double)cyclesNeeded);
                        break;

                    case FundFrequency.Weekly:
                        endDate = DateTime.Now.AddDays((double)cyclesNeeded * 7);
                        break;

                    case FundFrequency.Monthly:
                        int monthsNeeded = (int)Math.Ceiling(cyclesNeeded);
                        endDate = DateTime.Now.AddMonths(monthsNeeded);
                        break;

                    default:
                        throw new Exception("Invalid FundFrequency");
                }

                personalSavings.EndDate = endDate;
                personalSavings.WithdrawalDate = endDate.AddDays(1);

                // Set NextRuntime depending on AutoSave
                if (personalSavings.AutoSave)
                {
                    switch (personalSavings.FundFrequency)
                    {
                        case FundFrequency.Daily:
                            personalSavings.NextRuntime = DateTime.Now.AddDays(1);
                            break;

                        case FundFrequency.Weekly:
                            personalSavings.NextRuntime = DateTime.Now.AddDays(7);
                            break;

                        case FundFrequency.Monthly:
                            personalSavings.NextRuntime = DateTime.Now.AddMonths(1);
                            break;
                    }
                }
                else
                {
                    // For manual savings, we can set to a past date or nullable
                    personalSavings.NextRuntime = DateTime.Now.AddDays(-1);
                }

                _savingRepository.UpdateAsync(personalSavings);

                SetSuccessResponse(response, "Personal savings details updated successfully", personalSavings, 200);
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
                var result = _unitOfWork.WalletFundingRepository.GetAll();
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

        public async Task<ResponseDto<string>> DeletePersonalSavings(string personalSavingsId, string userId)
        {
            var response = new ResponseDto<string>();

            try
            {
                var user = await _userManager.FindByEmailAsync(userId);

                if (user == null)
                {
                    SetErrorResponse(response, "User not found", 404);
                    return response;
                }

                var personalSavings = await _savingRepository.GetSavingByIdAsync(personalSavingsId);

                if (personalSavings == null)
                {
                    SetNotFoundResponse(response, "Personal savings details not found", 404);
                    return response;
                }
                await _savingRepository.DeleteAsync(personalSavings);
                SetSuccessResponse(response, "Savings deleted successfully", 200);
            }
            catch (Exception ex)
            {
                SetErrorResponse(response, ex.Message, 500);
            }
            return response;
        }
    }
 }


