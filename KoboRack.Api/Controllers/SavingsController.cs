using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Model.Entities;

namespace KoboRack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavingsController : ControllerBase
    {
        private readonly IPersonalSavings _personalSavings;
        private readonly IMapper _mapper;
        public SavingsController(IPersonalSavings personalSavings, IMapper mapper)
        {
            _personalSavings = personalSavings;
            _mapper = mapper;
        }

        [HttpPost("SetGoal")]
        public async Task<IActionResult> AddMoreGoals([FromForm] PersonalSavingsDTO saving)
        {
            var response = await _personalSavings.SetPersonal_Savings_Target(saving, saving.UserId);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("listAllGoals/{UserId}")]
        public async Task<IActionResult> GetAllGoals(string UserId)
        {
            var response = await _personalSavings.Get_ListOf_All_UserTargets(UserId);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getPersonalSavings/{personalSavingsId}")]
        public async Task<IActionResult> GetPersonalSavingsById(string personalSavingsId)
        {
            var response = await _personalSavings.GetPersonalSavingsById(personalSavingsId);

            return response.StatusCode switch
            {
                200 => Ok(response),
                404 => NotFound(response),
                _ => BadRequest(response),
            };
        }

        [HttpGet("totalGoalAmount/{userId}")]
        public async Task<IActionResult> GetTotalGoalAmountByUser(string userId)
        {
            var response = await _personalSavings.GetTotalGoalAmountByUser(userId);

            return response.StatusCode switch
            {
                200 => Ok(response),
                404 => NotFound(response),
                _ => BadRequest(response),
            };
        }

        [HttpGet("totalPersonalFundCount")]
        public async Task<IActionResult> GetPersonalFundsCount()
        {
            var response = await _personalSavings.GetPersonalFundsCount();

            return response.StatusCode switch
            {
                200 => Ok(response),
                404 => NotFound(response),
                _ => BadRequest(response),
            };
        }
    }

}
