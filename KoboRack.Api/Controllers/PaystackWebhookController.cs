using KoboRack.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KoboRack.Api.Controllers
{
    [ApiController]
    [Route("api/webhook/paystack")]
    public class PaystackWebhookController : ControllerBase
    {
        private readonly IPaystackWebhookService _webhookService;

        public PaystackWebhookController(IPaystackWebhookService webhookService)
        {
            _webhookService = webhookService;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();
            var eventType = Request.Headers["x-paystack-event"];
            var signature = Request.Headers["x-paystack-signature"];

            var isVerified = _webhookService.VerifySignature(payload, signature);
            if (!isVerified)
                return Unauthorized();

            await _webhookService.HandleWebhookAsync(payload);
            return Ok();
        }
    }
}
