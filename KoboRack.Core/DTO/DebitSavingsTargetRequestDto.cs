using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.DTO
{
    public class DebitSavingsTargetRequestDto
    {
        public string WalletId { get; set; }
        public string SavingsGoalId { get; set; }
        public decimal Amount { get; set; }
    }
}
