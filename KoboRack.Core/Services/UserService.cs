using AutoMapper;
using Microsoft.AspNetCore.Identity;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Context;
using KoboRack.Model;
using KoboRack.Model.Entities;

namespace KoboRack.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly SaviDbContext _saviDbContext;

        public UserService(UserManager<AppUser> userManager, IMapper mapper,SaviDbContext saviDbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _saviDbContext = saviDbContext;
        }

        public async Task<ApiResponse<AppUserDto>> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                return user != null
                    ? SuccessResponse(_mapper.Map<AppUserDto>(user), "User found.")
                    : NotFoundResponse();
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        private ApiResponse<AppUserDto> NotFoundResponse()
            => ApiResponse<AppUserDto>.Failed(false, "User not found.", 404, new List<string> { "User not found." });

        private ApiResponse<AppUserDto> SuccessResponse(AppUserDto userDto, string message)
            => ApiResponse<AppUserDto>.Success(userDto, message, 200);

        private ApiResponse<AppUserDto> ErrorResponse(Exception ex)
            => ApiResponse<AppUserDto>.Failed(false, "An error occurred while retrieving the user.", 500, new List<string> { ex.Message });

        public ResponseDto<int> NewUserCountAsync()
        {
            try
            {
                var allUsers = _saviDbContext.Users.ToList();
                var newUsers = new List<AppUser>();
                foreach (var user in allUsers)
                {
                    if (user.CreatedAt.Date == DateTime.Today.Date)
                    {
                        newUsers.Add(user);
                    }
                }
                return new ResponseDto<int>
                {
                    DisplayMessage = $"{newUsers.Count} new members found",
                    Result = newUsers.Count,
                    StatusCode = 200
                };
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
