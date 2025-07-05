using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.Commons
{
    public class PaystackCustomerResult
    {
        public bool IsSuccess { get; set; }
        public string CustomerCode { get; set; }
        public string InternalId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class PaystackCustomerResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public PaystackCustomerData data { get; set; }
    }

    public class PaystackCustomerData
    {
        public int id { get; set; }
        public string customer_code { get; set; }
    }
}
