using KoboRack.Core.IServices;
using KoboRack.Data.Context;
using KoboRack.Model;
using KoboRack.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.Services
{
    public class UserValidationService : IUserValidationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UserValidationService> _logger;
        private readonly SaviDbContext _dbContext;

        public UserValidationService(UserManager<AppUser> userManager, ILogger<UserValidationService> logger, SaviDbContext dbContext)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user == null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email uniqueness");
                throw;
            }
        }

        public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
                return user == null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking phone number uniqueness");
                throw;
            }
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync();
                if (user == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking token validity");
                throw;
            }
        }
       
    }
}
