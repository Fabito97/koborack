using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Math.EC.Multiplier;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Repositories.Implementation;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using KoboRack.Model.Enums;
using System.Net;
using System.Text;

namespace KoboRack.Core.Services
{
    public class GroupSavingsMembersServices : IGroupSavingsMembersServices
    {
        private readonly IGroupSavingsMembersRepository _groupSavingsMembersRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GroupSavingsMembersServices> _logger;
        private readonly UserManager<AppUser> _userManager;



        public GroupSavingsMembersServices(IGroupSavingsMembersRepository groupSavingsMembersRepository, IUnitOfWork unitOfWork, ILogger<GroupSavingsMembersServices> logger, UserManager<AppUser> userManager)
        {
            _groupSavingsMembersRepository = groupSavingsMembersRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }


        public async Task<ResponseDto<bool>> JoinGroupSavingsAsync(string userId, string groupId)
        {
            try
            {
                var member = await _groupSavingsMembersRepository.GetUserByIdAsync(userId);
                if (member == null)
                {
                    return new ResponseDto<bool>()
                    {
                        DisplayMessage = "User not found",
                        StatusCode = 404,
                        Result = false
                    };
                }

                var memberExist = await _groupSavingsMembersRepository.CheckIfUserExist(userId, groupId);
                if (memberExist)
                {
                    return new ResponseDto<bool>()
                    {
                        DisplayMessage = "You are already a member",
                        StatusCode = 400,
                        Result = false
                    };
                }

                var group = await _unitOfWork.GroupRepository.GetGroupByIdAsync(groupId);
                if (group == null)
                {
                    return new ResponseDto<bool>()
                    {
                        DisplayMessage = "Group doesn't exist",
                        StatusCode = 400,
                        Result = false
                    };
                }

                var lastPosition = await _groupSavingsMembersRepository.GetGroupLastUserPosition(groupId);
                if (lastPosition == 4) 
                {
                     UpdateGroupDetails(group);
                }

                var isGroupFull = lastPosition >= group.MaxNumberOfParticipants ;
                if (isGroupFull)
                {
                    return new ResponseDto<bool>()
                    {
                        DisplayMessage = $"{group.SaveName} is already full",
                        StatusCode = 400,
                        Result = false
                    };
                }

                var newGroupMember = new GroupSavingsMembers
                {
                    Positions = lastPosition+1,
                    UserId = userId,
                    IsGroupOwner = false,
                    GroupSavingsId = groupId
                };

                var memberToAdd = await _groupSavingsMembersRepository.CreateSavingsGroupMembersAsync(newGroupMember);
                if (memberToAdd)
                {
                    UpdateGroupDetails1(group);
                    return new ResponseDto<bool>()
                    {
                        DisplayMessage = $"Successfully added to {group.SaveName} group",
                        StatusCode = 200,
                        Result = true
                    };
                }

                return new ResponseDto<bool>()
                {
                    DisplayMessage = $"Unable to add you to {group.SaveName}",
                    StatusCode = 404,
                    Result = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while joining group savings");
                return new ResponseDto<bool>()
                {
                    DisplayMessage = "An error occurred while processing your request",
                    StatusCode = 500,
                    Result = false
                };
            }
        }

        private void UpdateGroupDetails1(Group group)
        {
            group.MemberCount++;
            _unitOfWork.GroupRepository.UpdateAsync(group);
            _unitOfWork.SaveChanges();
        }


        private void UpdateGroupDetails(Group group)
        {
            group.ActualStartDate = DateTime.UtcNow.Date;
            group.NextRunTime = DateTime.Today;
            group.GroupStatus = GroupStatus.OnGoing;

            switch (group.Schedule)
            {
                case FundFrequency.Daily:
                    group.ExpectedEndDate = DateTime.UtcNow.AddDays(6);
                    break;
                case FundFrequency.Weekly:
                    group.ExpectedEndDate = DateTime.UtcNow.AddDays(35);
                    break;
                case FundFrequency.Monthly:                  
                    group.ExpectedEndDate = DateTime.UtcNow.AddDays(155);
                    break;
                default:
                    break;
            }

            //group.MemberCount++;
            _unitOfWork.GroupRepository.UpdateAsync(group);
            _unitOfWork.SaveChanges();
        }

        public async Task<ResponseDto<Dictionary<string, string>>> GetGroupMembersAsync(string groupId)
        {
            var groupExist =  _unitOfWork.GroupRepository.FindGroups(x=>x.Id == groupId);
            ArgumentNullException.ThrowIfNull(groupExist);
            var members = new Dictionary<string, string>();
            foreach(var group in groupExist) 
            {
                var users = await _userManager.FindByIdAsync(group.UserId);
                ArgumentNullException.ThrowIfNull(nameof(users));
                var Paid =  await _groupSavingsMembersRepository.FindAsync2(x=>x.UserId == group.UserId);
                var isPaid = Paid?.IsPaid ?? false;
                members.Add($"{users.FirstName} {users.LastName}", $"{(isPaid ? "Paid" : "UnPaid")}");
            } 
            if (members.Count == 0)
            {
                return new ResponseDto<Dictionary<string, string>>()
                {
                    DisplayMessage = $"No members found for group with ID {groupId}",
                    StatusCode = 201,
                    Result = null
                };
            }
            return new ResponseDto<Dictionary<string, string>>()
            {
                DisplayMessage = $"{members.Count} {(members.Count == 1 ? "Member" : "Members")} found",
                Result = members,
                StatusCode = 200
            };
        }

    }
}
