using KoboRack.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KoboRack.Core.IServices
{
    public interface ITokenService
    {
        string GenerateOtp(string userId, int length = 6);
        JwtSecurityToken GetToken(List<Claim> authClaims);
        Task<ApiResponse<string>> ConfirmOtpToken(string userId, string token);
    }
}