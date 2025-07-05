using KoboRack.Core.DTO.PaystackDto;

namespace KoboRack.Core.DTO
{
	public class CreditResponseDto
	{
		public string WalletNumber { get; set; }
		public decimal Balance { get; set; }
		public string Message { get; set; }
		public string PaymentUrl {  get; set; }
		public string AccessCode { get; set; }
		public string Reference { get; set; }
	}
}
