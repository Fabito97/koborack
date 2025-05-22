using System.ComponentModel.DataAnnotations.Schema;

namespace KoboRack.Model.Entities
{
    public class CardDetail : BaseEntity
    {
        public string NameOnCard { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public string CVV { get; set; } = string.Empty;

        [ForeignKey("AppUserId")]
        public string AppUserId { get; set; }
    }
}
