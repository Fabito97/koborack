using KoboRack.Core.IServices;
using KoboRack.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly UserManager<AppUser> _userManager;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            var user = httpContextAccessor.HttpContext?.User;
            UserId = user?.FindFirstValue(ClaimTypes.NameIdentifier); // maps to "nameidentifier"
            UserRole = user?.FindFirstValue(ClaimTypes.Role); // maps to "role"
            UserEmail = user?.FindFirstValue(ClaimTypes.Name); // maps to "name"

            FullName = httpContextAccessor.HttpContext?.User?.FindFirstValue("FirstName")
                       + " " + httpContextAccessor.HttpContext?.User?.FindFirstValue("LastName");
            UserPhoneNumber = httpContextAccessor.HttpContext?.User?.FindFirstValue("PhoneNumber");
            UserType = httpContextAccessor.HttpContext?.User?.FindFirstValue("UserType");

            _userManager = userManager;
        }

        public string? UserId { get; }
        public string? UserRole { get; }
        public string? UserEmail { get; }
        public string FullName { get; }
        public string? UserPhoneNumber { get; set; }
        public string? UserType { get; set; }

        public async Task<AppUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(UserId);
        }
    }
}
