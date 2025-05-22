using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model;

namespace KoboRack.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IMapper mapper, IUnitOfWork unitOfWork, ILogger<AdminService> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public ApiResponse<GroupDTO> GetGroupSavingById(string groupId)
        {
            try
            {
                var group = _unitOfWork.GroupRepository.GetById(groupId);
                if (group == null)
                {
                    return new ApiResponse<GroupDTO>(false, "Group not found", StatusCodes.Status404NotFound);
                }
                var response = _mapper.Map<GroupDTO>(group);
                return new ApiResponse<GroupDTO>(true, "Group found successfully", StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting Group Saving by Id");
                return new ApiResponse<GroupDTO>(false, "Error occurred while processing your request", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
        }

        public Task<ApiResponse<GroupTransactionDto>> GetGroupTransactionsAsync(int page, int perPage)
        {
            throw new NotImplementedException();
        }
    }
}
