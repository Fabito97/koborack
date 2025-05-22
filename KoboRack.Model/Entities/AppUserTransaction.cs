using KoboRack.Model.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoboRack.Model.Entities
{
    public class AppUserTransaction : BaseEntity
    {
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;

        [ForeignKey("AppUserId")]
        public string AppUserId { get; set; }
    }
}
