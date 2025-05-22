using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KoboRack.Core.IServices;
using KoboRack.Core.Services;
using KoboRack.Model;

namespace KoboRack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("groupId")]
        public IActionResult GetGroupSaving(string groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }
            return Ok(_adminService.GetGroupSavingById(groupId));
        }
    }
}
