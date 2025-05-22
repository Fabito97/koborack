using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Core.Services;

namespace KoboRack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundingController : ControllerBase
    {
        private readonly IFundingService _fundingService;

        public FundingController(IFundingService fundingService)
        {
            _fundingService = fundingService;
        }
        [HttpPost("CreditPersonalTarget")]
        public async Task<IActionResult> CreditPersonalTarget([FromBody] CreditSavingsTargetRequestDto request)
        {
            try
            {
                bool result = await _fundingService.CreditPersonalTarget(request.WalletId, request.SavingsGoalId, request.Amount);

                if (!result)
                {
                    return BadRequest("Unable to credit the personal target.");
                }

                return Ok("Personal target credited successfully.");
            }
            catch (Exception ex)
            {               
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPost("DebitPersonalTarget")]
        public async Task<IActionResult> DebitPersonalTarget([FromBody] DebitSavingsTargetRequestDto request)
        {
            try
            {
                bool result = await _fundingService.DebitPersonalTarget(request.WalletId, request.SavingsGoalId, request.Amount);               

                if (!result)
                {
                    return BadRequest("Unable to debit the personal target.");
                }
                return Ok("Personal target debited successfully.");
            }
            catch (Exception ex)
            {               
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
