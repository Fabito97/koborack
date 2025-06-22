using KoboRack.Model;
using KoboRack.Model.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KoboRack.Core.IServices
{
    public interface ITokenService
    {
        string GenerateOtp(string userId, int length = 6);
        Task<string> GetToken(AppUser user);
        Task<ApiResponse<string>> ConfirmOtpToken(string userId, string token);
    }
}