using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.Commons
{
    public class PaystackDvaResult
    {
        public bool IsSuccess { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string ReferenceId { get; set; } // Optional Paystack internal ID
        public string ErrorMessage { get; set; }
    }
}
