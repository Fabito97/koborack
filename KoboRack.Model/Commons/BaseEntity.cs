using System.ComponentModel.DataAnnotations.Schema;

namespace KoboRack.Model.Commons
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
    }
}
