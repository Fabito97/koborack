using System.ComponentModel.DataAnnotations.Schema;

namespace KoboRack.Model.Entities
{
    public class Otp : BaseEntity
    {
        public long Value { get; set; }
        public bool IsUsed { get; set; }

        [ForeignKey("AppUserId")]
        public string AppUserId { get; set; }
    }
}