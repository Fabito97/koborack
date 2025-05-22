using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using KoboRack.Model.Enums;
using System;
using static Google.Apis.Requests.BatchRequest;

namespace KoboRack.Core.Services
{
    public class GroupSavings : IGroupSavings
    {
        private readonly IMapper _IMapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SaviDbContext _saviDbContext;
        private readonly IGroupSavingsMembersRepository _groupSavingsMembersRepository;
        private readonly ICloudinaryServices<Group> _cloudinaryServices;

        public GroupSavings(IMapper iMapper, IUnitOfWork unitOfWork, ICloudinaryServices<Group> cloudinaryServices, SaviDbContext saviDbContext, IGroupSavingsMembersRepository groupSavingsMembersRepository)
        {
            _IMapper = iMapper;
            _unitOfWork = unitOfWork;
            _cloudinaryServices = cloudinaryServices;
            _saviDbContext = saviDbContext;
            _groupSavingsMembersRepository = groupSavingsMembersRepository;
        }

        public ResponseDto<List<GroupDTO>> GetExploreGroupSavingGroups()
        {
            var response = new ResponseDto<List<GroupDTO>>();

            try
            {
                //var exploreGroupSavingGroups =  _unitOfWork.GroupRepository.GetAll();
                //var waitingGroups = exploreGroupSavingGroups.Where(groupDto => groupDto.GroupStatus == GroupStatus.Waiting).ToList();
                var waitingGroups = _unitOfWork.GroupRepository.GetAll(groupDto => groupDto.GroupStatus == GroupStatus.Waiting);

                if (waitingGroups.Count > 0)
                {
                    var mappedExploreGroups = _IMapper.Map<List<GroupDTO>>(waitingGroups);
                    return new ResponseDto<List<GroupDTO>>()
                    {
                        DisplayMessage = "Success",
                        Result = mappedExploreGroups,
                        StatusCode = 200
                    };                    
                }
                else
                {                    
                    return new ResponseDto<List<GroupDTO>>()
                    {
                        DisplayMessage = "No explore savings group accounts found",
                        Result = null,
                        StatusCode = 404
                    };
                }
            }
            catch (Exception ex)
            {                
                return new ResponseDto<List<GroupDTO>>()
                {
                    DisplayMessage = $"{ex.Message}",
                    Result = null,
                    StatusCode = 500
                };
            }        
        }

        public ResponseDto<GroupDTO> GetExploreGroupSavingDetails(string groupId)
        {
            ArgumentNullException.ThrowIfNull(nameof(groupId));
            try
            {
                var group = _unitOfWork.GroupRepository.GetById(groupId);
                ArgumentNullException.ThrowIfNull(group);
                if (group.GroupStatus == GroupStatus.Waiting)
                {
                    var mappedExploreGroup = _IMapper.Map<GroupDTO>(group);
                    return new ResponseDto<GroupDTO>()
                    {
                        DisplayMessage = "Success",
                        Result = mappedExploreGroup,
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ResponseDto<GroupDTO>()
                    {
                        DisplayMessage = $"{group.SaveName} has been updated to {group.GroupStatus} status.",
                        Result = null,
                        StatusCode = 404
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<GroupDTO>()
                {
                    DisplayMessage = $"{ex.Message}",
                    Result = null,
                    StatusCode = 500
                };
            }
        }

        public ResponseDto<GroupDTO> GetGroupSavingAccountDetails(string groupId)
        {
            ArgumentNullException.ThrowIfNull(nameof(groupId));
            try
            {
                var group = _unitOfWork.GroupRepository.GetById(groupId);
                ArgumentNullException.ThrowIfNull(group);
                if (group.GroupStatus == GroupStatus.OnGoing)
                {
                    var mappedExploreGroup = _IMapper.Map<GroupDTO>(group);
                    return new ResponseDto<GroupDTO>()
                    {
                        DisplayMessage = "Success",
                        Result = mappedExploreGroup,
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ResponseDto<GroupDTO>()
                    {
                        DisplayMessage = $"{group.SaveName} has been updated to {group.GroupStatus} status.",
                        Result = null,
                        StatusCode = 404
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<GroupDTO>()
                {
                    DisplayMessage = $"{ex.Message}",
                    Result = null,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDto<string>> CreateSavingsGroup(GroupDTO2 dto)
        {
            try
            {
                var userExist = _saviDbContext.Users.FirstOrDefault(x => x.Id == dto.UserId);
                ArgumentNullException.ThrowIfNull(nameof(userExist));
                var nameExist = _saviDbContext.Groups.FirstOrDefault(x=> x.SaveName == dto.SaveName);
                if (nameExist != null)
                {
                    return new ResponseDto<string>()
                    {
                        DisplayMessage = $"{nameExist.SaveName} already exist",
                        StatusCode = 403,
                        Result = null,
                    };
                }
                var newGroupSavings = _IMapper.Map<Group>(dto);
                newGroupSavings.MemberCount = 1;
                newGroupSavings.MaxNumberOfParticipants = 5;
                newGroupSavings.GroupStatus = GroupStatus.Waiting;
                var result = await _unitOfWork.GroupRepository.AddGroupAsync2(newGroupSavings);            
                if (result != null)
                {
                    var newGroupMember = new GroupSavingsMembers()
                    {
                        Positions = 1,
                        IsGroupOwner = true,
                        IsPaid = false,
                        GroupSavingsId = newGroupSavings.Id,
                        UserId = dto.UserId
                    };
                    var res = await _groupSavingsMembersRepository.CreateSavingsGroupMembersAsync(newGroupMember);
                    var imageUrl = await _cloudinaryServices.UploadImage(result.Id, dto.FormFile);
                    result.SafePortraitImageURL = imageUrl;
                    _unitOfWork.GroupRepository.UpdateGroupAsync(result);
                    _unitOfWork.SaveChanges();
                    if (res)
                    {
                        return new ResponseDto<string>()
                        {
                            DisplayMessage = $"{result.SaveName} Group created successfully",
                            StatusCode = 200,
                            Result = null,
                        };
                    }
                }
                return new ResponseDto<string>()
                {
                    DisplayMessage = "Failed to create Group",
                    StatusCode = 400,
                    Result = null,
                };
            }
            catch (Exception ex)
            {

                return new ResponseDto<string>()
                {
                    DisplayMessage = $"{ex.Message}",
                    Result = null,
                    StatusCode = 500
                };
            }
        }

        public ResponseDto<List<GroupDTO>> GetListOfActiveSavingsGroups()
        {
            try
            { 
                var ongoingGroups = _unitOfWork.GroupRepository.GetAll(groupDto => groupDto.GroupStatus == GroupStatus.OnGoing);

                if (ongoingGroups.Count > 0)
                {
                    var mappedActiveGroups = _IMapper.Map<List<GroupDTO>>(ongoingGroups);
                    return new ResponseDto<List<GroupDTO>>()
                    {
                        DisplayMessage = "Success",
                        Result = mappedActiveGroups,
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ResponseDto<List<GroupDTO>>()
                    {
                        DisplayMessage = "No Active savings group found",
                        Result = null,
                        StatusCode = 404
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<GroupDTO>>()
                {
                    DisplayMessage = $"{ex.Message}",
                    Result = null,
                    StatusCode = 500
                };
            }

        }

        public ResponseDto<int> GetTotalSavingsGroupCountAsync()
        {
            try
            {
                var result = _unitOfWork.SavingRepository.GetAll();
                return new ResponseDto<int>()
                {
                    DisplayMessage = $"{result.Count} Personal group found",
                    Result = result.Count,
                    StatusCode = 200,
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<int>()
                {
                    DisplayMessage = $"{ex.Message}",
                    Result = 0,
                    StatusCode = 200,
                };
            }
        }
        public ResponseDto<int> GetNewGroupCount()
        {
            try
            {
                var allGroups = _unitOfWork.GroupRepository.GetAll();
                var newGroups = new List<Group>();
                foreach (var group in allGroups)
                {
                    if (group.CreatedAt.Date == DateTime.Today.Date)
                    {
                        newGroups.Add(group);
                    }
                }
                return new ResponseDto<int>
                {
                    DisplayMessage = $"{newGroups.Count} created today",
                    StatusCode = 200,
                    Result = newGroups.Count,
                };
            }
            catch (Exception ex)
            {

                return new ResponseDto<int>
                {
                    DisplayMessage = $"{ex.Message}",
                    StatusCode = 500,
                };
            }
        }
        public ResponseDto<List<GroupDTO>> GetAllGroups()
        {
            try
            {
                var exploreGroupSavingGroups = _unitOfWork.GroupRepository.GetAll();
                if (exploreGroupSavingGroups.Count > 0)
                {
                    var mappedGroups = _IMapper.Map<List<GroupDTO>>(exploreGroupSavingGroups);
                    return new ResponseDto<List<GroupDTO>>()
                    {
                        DisplayMessage = "Success",
                        Result = mappedGroups,
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ResponseDto<List<GroupDTO>>()
                    {
                        DisplayMessage = "No Groups Created",
                        Result = null,
                        StatusCode = 404
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<GroupDTO>>()
                {
                    DisplayMessage = $"{ex.Message}",
                    Result = null,
                    StatusCode = 500
                };
            }
        }
        public ResponseDto<List<GroupDTO>> GetGroupsCreatedToday()
        {
            var response = new ResponseDto<List<GroupDTO>>();

            try
            {
                var waitingGroups = _unitOfWork.GroupRepository.GetAll(groupDto => groupDto.CreatedAt.Date == DateTime.Today.Date);

                    var mappedGroups = _IMapper.Map<List<GroupDTO>>(waitingGroups);
                    return new ResponseDto<List<GroupDTO>>()
                    {
                        DisplayMessage = "Success",
                        Result = mappedGroups,
                        StatusCode = 200
                    };
           
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<GroupDTO>>()
                {
                    DisplayMessage = $"{ex.Message}",
                    Result = null,
                    StatusCode = 500
                };
            }
        }
        public ResponseDto<GroupDTO> GetGroupDetails(string groupId)
        {
            ArgumentNullException.ThrowIfNull(nameof(groupId));
            try
            {
                var group = _unitOfWork.GroupRepository.GetById(groupId);
                ArgumentNullException.ThrowIfNull(group);

                var mappedExploreGroup = _IMapper.Map<GroupDTO>(group);
                return new ResponseDto<GroupDTO>()
                {
                    DisplayMessage = "Success",
                    Result = mappedExploreGroup,
                    StatusCode = 200
                };
                
             
            }
            catch (Exception ex)
            {
                return new ResponseDto<GroupDTO>()
                {
                    DisplayMessage = $"{ex.Message}",
                    Result = null,
                    StatusCode = 500
                };
            }
        }

    }
}
    

