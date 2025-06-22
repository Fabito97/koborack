using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerBI.Api.Models;
using KoboRack.Core.IServices;
using KoboRack.Model;

namespace KoboRack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {
            var response = await _userService.GetUserByIdAsync(userId);

            if (response.Succeeded)
            {
                return Ok(response.Data);
            }

            return StatusCode(response.StatusCode, new { errors = response.Errors });
        }

        [HttpPut("updateUserInformation")]
        public async Task<IActionResult> UpdateUserInformation(string userId, IFormFile formFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }
            var response = await _userService.UpdateUserInformation(userId, formFile);

            if (!response.Succeeded)
                return StatusCode(response.StatusCode, response);

            return Ok(response);
        }

        [HttpGet("NewRegisteredUserCount")]
        public IActionResult NewRegisteredUserCount()
        {
            var response =  _userService.NewUserCountAsync();

            if (response.StatusCode == 200)
            {
                return Ok(response.Result);
            }

            return BadRequest(response.StatusCode);
        }
    }
}
