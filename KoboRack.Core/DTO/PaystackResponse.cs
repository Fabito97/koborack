namespace KoboRack.Core.DTO
{
    public class PaystackResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public PaystackData Data { get; set; }        
    }
    public class PaystackData
    {
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public PaystackCustomer Customer { get; set; }
    }
    public class PaystackCustomer
    {
        public string Email { get; set; }
    }
}
