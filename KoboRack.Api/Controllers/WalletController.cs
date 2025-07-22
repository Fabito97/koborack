using Microsoft.AspNetCore.Mvc;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Core.Services;
using KoboRack.Model;
using Microsoft.AspNetCore.Authorization;

namespace KoboRack.Api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletServices _walletServices;
		private readonly IWalletService _walletService;

        public WalletController(IWalletServices walletServices, IWalletService walletService)
        {
            _walletServices = walletServices;
			_walletService = walletService;
        }

        [HttpPost]
        [Route("paystack/verify/{referenceCode}")]
        public async Task<IActionResult> VerifyPayment(string referenceCode)
        {
            var result = await _walletServices.VerifyTransaction(referenceCode);

            if (!result.Succeeded) 
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {
            var response =  _walletServices.GetUserWalletAsync(userId);
            return Ok(response);
        }

		[HttpGet("GetAllWallets")]
		public async Task<IActionResult> AllWallets()
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse<string>.Failed(false, "Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
			}

			return Ok(await _walletService.GetAllWallets());
		}

		[HttpGet("GetWalletByNumber")]
		public async Task<IActionResult> GetWalletByNumber(string number)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse<string>.Failed(false, "Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
			}

			return Ok(await _walletService.GetWalletByNumber(number));
		}

        
		[HttpPost("FundWallet")]
		public async Task<IActionResult> FundWallet(FundWalletDto fundWalletDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse<string>.Failed(false, "Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
			}

			return Ok(await _walletService.FundWalletAsync(fundWalletDto));
		}

        [HttpGet("totalbalance")]
        public async Task<IActionResult> GetTotalCustomerWalletBalanceAsync()
        {
            var response = await _walletServices.GetTotalCustomerWalletBalanceAsync();
            return StatusCode(response.StatusCode, response);
        }
    }
}
