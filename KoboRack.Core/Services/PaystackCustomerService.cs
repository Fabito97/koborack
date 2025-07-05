using KoboRack.Core.Commons;
using KoboRack.Core.IServices;
using KoboRack.Model.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PayStack.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KoboRack.Model.Enums;
using KoboRack.Core.DTO.PaystackDto;

namespace KoboRack.Core.Services
{
    public class PaystackCustomerService : IPaystackCustomerService
    {
        private readonly PayStackApi _paystack;
        private readonly ILogger<PaystackCustomerService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public PaystackCustomerService(IConfiguration config, ILogger<PaystackCustomerService> logger)
        {
            _secretKey = config.GetValue<string>("PaystackApi:SecretKey");
            _paystack = new PayStackApi(_secretKey);
            _logger = logger;
            _httpClient = new();
        }

        public async Task<PaystackCustomerResult> CreateCustomerAsync(AppUser user, string walletNumber)
        {
            try
            {
                // Set up HTTP client with auth
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.paystack.co/customer");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);

                // Build metadata & payload
                var payload = new
                {
                    first_name = user.FirstName,
                    last_name = user.LastName,
                    email = user.Email,
                    phone = user.PhoneNumber,
                    metadata = new
                    {
                        account_alias = walletNumber,
                        user_id = user.Id,
                        //custom_fields = new[]
                        //{
                        //    new 
                        //    {
                        //        display_name = "Wallet Number",
                        //        variable_name = "account_alias",
                        //        value = walletNumber
                        //    }
                        //}
                    }
                };

                var json = JsonConvert.SerializeObject(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send request
                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Paystack returned error creating customer: {StatusCode} - {Body}", response.StatusCode, responseBody);
                    return new PaystackCustomerResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Paystack error: {responseBody}"
                    };
                }

                // Deserialize cleanly
                var parsed = JsonConvert.DeserializeObject<PaystackCustomerResponse>(responseBody);

                return new PaystackCustomerResult
                {
                    IsSuccess = true,
                    CustomerCode = parsed.data.customer_code,
                    InternalId = parsed.data.id.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Paystack customer creation failed");
                return new PaystackCustomerResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<PaystackDvaResult> CreateDedicatedAccountAsync(string paystackCustomerCode)
        {
            try
            {
                var payload = new
                {
                    customer = paystackCustomerCode,
                    preferred_bank = "wema-bank"
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.paystack.co/dedicated_account")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);

                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(content);

                if (response.IsSuccessStatusCode && json.status == true)
                {
                    return new PaystackDvaResult
                    {
                        IsSuccess = true,
                        AccountNumber = json.data.account_number,
                        BankName = json.data.bank.name,
                        AccountName = json.data.account_name,
                        ReferenceId = json.data.id
                    };
                }

                return new PaystackDvaResult
                {
                    IsSuccess = false,
                    ErrorMessage = json.message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DVA creation failed.");
                return new PaystackDvaResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<PaystackInitResponseDto> InitiateWalletFundingAsync(AppUser user, AppUserTransaction walletTransaction, string customerCode)
        {
            var reference = walletTransaction.Reference;

            var payload = new
            {
                email = user.Email,
                amount = (int)(walletTransaction.Amount * 100), // Kobo
                reference = reference,
                metadata = new
                {
                    userId = user.Id,
                    walletId = walletTransaction.Id,
                    transactionId = walletTransaction.Id,
                    customerCode
                },
                callback_url = "https://koborack.com/api/paystack/callback" // optional
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.paystack.co/transaction/initialize")
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(content);

            if ((bool)result.status)
            {
                return new PaystackInitResponseDto
                {
                    AuthorizationUrl = result.data.authorization_url,
                    AccessCode = result.data.access_code,
                    Reference = result.data.reference
                };
            }

            throw new Exception($"Paystack Init Error: {result.message}");
        }
    }
}
