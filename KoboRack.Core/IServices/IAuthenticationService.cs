using KoboRack.Core.DTO;
using KoboRack.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KoboRack.Model.Entities;
using Microsoft.AspNetCore.Http;

namespace KoboRack.Core.IServices
{
    public interface IAuthenticationService
    {
        Task<ApiResponse<string>> ForgotPasswordAsync(string email);
        Task<ApiResponse<string>> ResetPasswordAsync(string email, string token, string newPassword);
        Task<ApiResponse<string>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
        Task<ApiResponse<string>> LoginAsync(AppUserLoginDTO loginDTO);
        JwtSecurityToken GetToken(List<Claim> authClaims);
        Task<ApiResponse<string>> RegisterAsync(AppUserCreateDto appUserCreateDto);
        Task<ApiResponse<string>> VerifyAndAuthenticateUserAsync(string idToken);
        Task<ApiResponse<string>> ConfirmEmailAsync(string userId, string token);
        Task<ApiResponse<string>> ResendEmailVerifyLink(string userId);
        Task<ApiResponse<string>> UpdateUserInformation(string userId, IFormFile formFile);
    }
}
