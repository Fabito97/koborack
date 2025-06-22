using KoboRack.Core.IServices;
using KoboRack.Data.Context;
using KoboRack.Model;
using KoboRack.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;

        private readonly SaviDbContext _dbContext;

        public TokenService(IConfiguration config, SaviDbContext dbContext, UserManager<AppUser> userManager)
        {
            _config = config;
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<string> GetToken(AppUser user)
        {
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, role),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:ValidIssuer"],
                audience: _config["JwtSettings:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateOtp(string userId, int length = 6)
        {
            if (length < 4 || length > 10)
                throw new ArgumentException("OTP length must be between 4 and 10.");

            var otp = new StringBuilder(length);
            var randomBytes = new byte[length];
            RandomNumberGenerator.Fill(randomBytes); // .NET 6+

            foreach (byte b in randomBytes)
            {
                otp.Append((b % 10).ToString());
            }
           
            return otp.ToString();
        }

        public static string HashOtp(string otp)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(otp);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public async Task<ApiResponse<string>> ConfirmOtpToken(string userId, string token)
        {
            var otp = await _dbContext.Otps.FirstOrDefaultAsync(o => o.AppUserId == userId);

            if (otp == null)
            {
                return ApiResponse<string>.Failed(false, "Unathorized, Otp token not found", 401, null);
            }

            // Check expiry: OTP older than 10 minutes
            if (DateTime.UtcNow - otp.CreatedAt > TimeSpan.FromMinutes(10))
            {
                return ApiResponse<string>.Failed(false, "OTP expired", 401, null);
            }

            // Hash the incoming token and compare
            var hashedToken = HashOtp(token);
            if (!string.Equals(otp.Value, hashedToken, StringComparison.Ordinal))
            {
                return ApiResponse<string>.Failed(false, "Invalid OTP", 401, null);
            }

            // Optional: delete OTP after successful use
            _dbContext.Otps.Remove(otp);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<string>.Success(token, "OTP verified", 200);
        }

    }
}
