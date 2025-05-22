using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Model.Entities
{
    public class GroupSavingsMembers : BaseEntity
    {
        public string UserId { get; set; }

        public string GroupSavingsId { get; set; }

        public bool IsGroupOwner { get; set; }

        public int Positions { get; set; }

        public DateTime LastsavingDate { get; set; }
        public bool IsPaid { get; set; }

    }
}
