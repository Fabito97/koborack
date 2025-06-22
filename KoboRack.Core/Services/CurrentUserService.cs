using KoboRack.Core.IServices;
using Microsoft.AspNetCore.Http;
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
        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue("Id");
            UserRole = httpContextAccessor.HttpContext?.User?.FindFirstValue("Role");
            UserEmail = httpContextAccessor.HttpContext?.User?.FindFirstValue("Email");
            FullName = httpContextAccessor.HttpContext?.User?.FindFirstValue("FirstName")
                       + " " + httpContextAccessor.HttpContext?.User?.FindFirstValue("LastName");
            UserPhoneNumber = httpContextAccessor.HttpContext?.User?.FindFirstValue("PhoneNumber");
            UserType = httpContextAccessor.HttpContext?.User?.FindFirstValue("UserType");
        }

        public string? UserId { get; }
        public string? UserRole { get; }
        public string? UserEmail { get; }
        public string FullName { get; }
        public string? UserPhoneNumber { get; set; }
        public string? UserType { get; set; }
    }
}
