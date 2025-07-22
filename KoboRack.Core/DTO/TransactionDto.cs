using KoboRack.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.DTO
{
    public class TransactionDto
    {
        public string TransactionType { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string CreatedAt { get; set; }
    }
}
