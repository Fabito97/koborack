using AutoMapper;
using Microsoft.AspNetCore.Identity;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Context;
using KoboRack.Model;
using KoboRack.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KoboRack.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly SaviDbContext _saviDbContext;
        private readonly ILogger<UserService> _logger;
        private readonly ICloudinaryServices<AppUser> _cloudinaryServices;

        public UserService(UserManager<AppUser> userManager, IMapper mapper,SaviDbContext saviDbContext, ICloudinaryServices<AppUser> cloudinaryServices, ILogger<UserService> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _saviDbContext = saviDbContext;
            _cloudinaryServices = cloudinaryServices;
            _logger = logger;
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

        public async Task<ApiResponse<string>> UpdateUserInformation(string userId, IFormFile formFile)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(nameof(userId));
                ArgumentNullException.ThrowIfNull(nameof(formFile));

                var userExist = await _saviDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (userExist == null)
                {
                    return new ApiResponse<string>(false, "User not found", StatusCodes.Status404NotFound);
                }

                var image = await _cloudinaryServices.UploadImage(userId, formFile);

                // Update user's image URL
                userExist.ImageUrl = image;

                var result = await _userManager.UpdateAsync(userExist);

                if (result.Succeeded)
                {
                    return new ApiResponse<string>(true, "Account Updated successfully", StatusCodes.Status200OK);
                }
                return new ApiResponse<string>(false, "Failed to update account", StatusCodes.Status400BadRequest);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Argument is null");
                return new ApiResponse<string>(false, "Argument is null", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user");
                return new ApiResponse<string>(false, "Error occurred while updating user", StatusCodes.Status500InternalServerError);
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
