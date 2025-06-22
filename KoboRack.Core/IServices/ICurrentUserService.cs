using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.IServices
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserRole { get; }
        string? UserEmail { get; }
        string? UserPhoneNumber { get; set; }
        string FullName { get; }
        string? UserType { get; set; }
    }
}
