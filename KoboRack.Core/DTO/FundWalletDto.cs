using System.ComponentModel.DataAnnotations;

namespace KoboRack.Core.DTO
{
	public class FundWalletDto
	{

		[Required]
		public string WalletNumber { get; set; }
		public decimal FundAmount { get; set; }
		public string Naration { get; set; } = string.Empty;
	}
}
