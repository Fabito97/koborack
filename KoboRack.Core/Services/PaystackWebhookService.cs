using Google;
using KoboRack.Core.IServices;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Model.Entities;
using KoboRack.Model.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KoboRack.Core.Services
{
    public class PaystackWebhookService : IPaystackWebhookService
    {
        public string SecretKey { get; set; }
        private readonly IUnitOfWork _unitOfWork;

        public PaystackWebhookService(IConfiguration config, IUnitOfWork unitOfWork)
        {
            SecretKey = config.GetValue<string>("Paystack:SecretKey");
            _unitOfWork = unitOfWork;
        }

        public bool VerifySignature(string payload, string signature)
        {
            var secret = SecretKey;
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var calculated = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return signature == calculated;
        }

        
            public async Task<(Wallet wallet, AppUserTransaction transaction)> VerifyTransactionAsync(dynamic data)
            {
                string reference = data.reference;
                decimal amount = (decimal)data.amount / 100;
                string paystackCustomerCode = data.customer?.customer_code;
                string metadataCustomerCode = data.metadata?.customerCode;

                if (string.IsNullOrWhiteSpace(reference))
                    throw new ArgumentException("Reference missing from webhook data");

                var transaction = _unitOfWork.AppUserTransactionRepository
                    .Find(t => t.Reference == reference && t.Amount == amount).FirstOrDefault();

                if (transaction == null)
                    throw new InvalidOperationException("Transaction not found or already processed");

                var wallet = _unitOfWork.WalletRepository
                    .Find(w => w.Id == transaction.WalletId).FirstOrDefault();

                if (wallet == null)
                    throw new InvalidOperationException("Wallet linked to transaction not found");
                
            if (metadataCustomerCode != null && 
                !string.Equals(wallet.PaystackCustomerCode, paystackCustomerCode, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Customer code mismatch between metadata and wallet");
                           
                return (wallet, transaction);
            }
        

        public async Task HandleWebhookAsync(dynamic payload)
        {
            string eventType = payload.@event;

            switch (eventType)
            {
                case "charge.success":
                    await HandleChargeSuccessAsync(payload.data);
                    break;

                case "transfer.success":
                    await HandleTransferSuccessAsync(payload.data);
                    break;

                //case "dedicated_account.assigned":
                //    await HandleDvaAssignedAsync(payload.data);
                //    break;

                //case "transfer.failed":
                //    await HandleTransferFailureAsync(payload.data);
                //    break;

                // Add more handlers as needed...

                default:
                    // Log or ignore unknown event
                    break;
            }
        }

        private async Task HandleChargeSuccessAsync(dynamic data)
        {
            var result = await VerifyTransactionAsync(data);
            var wallet = result.wallet as Wallet;
            var transaction = result.transaction as AppUserTransaction;

            if (transaction.Status == TransactionStatus.Success)
            {
                Console.WriteLine("Transaction already processed");
                return;
            }

            transaction.Status = TransactionStatus.Success;
            _unitOfWork.AppUserTransactionRepository.UpdateAsync(transaction);
            await _unitOfWork.AppUserTransactionRepository.SaveChangesAsync();

            wallet.Balance += transaction.Amount;
            _unitOfWork.WalletRepository.UpdateAsync(wallet);
            await _unitOfWork.WalletRepository.SaveChangesAsync();
        }

        private async Task HandleTransferSuccessAsync(dynamic data)
        {
            string customerCode = data.customer?.customer_code;
            decimal amount = (decimal)data.amount / 100;

            // Lookup wallet by Paystack customer code
            var wallet = _unitOfWork.WalletRepository.Find(w => w.PaystackCustomerCode == customerCode);
            if (wallet == null) return;

            //await _walletService.CreditFromTransferAsync(wallet, amount, data.reference);
        }

    }
}
