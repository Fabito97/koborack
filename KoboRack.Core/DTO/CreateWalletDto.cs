using System.ComponentModel.DataAnnotations.Schema;

namespace KoboRack.Core.DTO
{
	public class CreateWalletDto
	{
		public string PhoneNumber { get; set; } = string.Empty;

		[ForeignKey("AppUserId")]
		public string UserId { get; set; } = string.Empty;
	}
}
