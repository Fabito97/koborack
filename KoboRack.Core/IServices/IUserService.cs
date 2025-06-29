﻿using KoboRack.Core.DTO;
using KoboRack.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.IServices
{
    public interface IUserService
    {
        Task<ApiResponse<AppUserDto>> GetUserByIdAsync(string userId);
        ResponseDto<int> NewUserCountAsync();
        Task<ApiResponse<string>> UpdateUserInformation(string userId, IFormFile formFile);
    }
}
