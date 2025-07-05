namespace KoboRack.Core.IServices
{
    public interface IPaystackWebhookService
    {

        Task HandleWebhookAsync(dynamic payload);
        bool VerifySignature(string payload, string signature);
    }
}