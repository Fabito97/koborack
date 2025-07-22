using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.DTO
{
    public class CreditSavingsTargetRequestDto
    {
        public string WalletId { get; set; }
        public string SavingsGoalId { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
