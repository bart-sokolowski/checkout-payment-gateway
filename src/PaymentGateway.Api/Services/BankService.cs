using System.Net;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services
{
    public class BankService
    {
        private readonly HttpClient _httpClient;
        public BankService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BankValidationResponse> ProcessPaymentAsync(PostPaymentRequest request)
        {

            //request body
            var bankPaymentRequest = new
            {
                card_number = request.CardNumber,
                expiry_date = $"{request.ExpiryMonth:D2}/{request.ExpiryYear}",
                currency = request.Currency,
                amount = request.Amount,
                cvv = request.Cvv
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/payments", bankPaymentRequest);


                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PostBankPaymentResponse>();

                    if(result != null)
                    {
                        return new BankValidationResponse
                        {
                            Success = true,
                            Authorized = result.Authorized,
                            AuthorizationCode = result.AuthorizationCode
                        };
                    }

                }

            }
            catch (Exception ex)
            {
                // Log exception or add a failed payment ticket (not implemented here as for demo)
            }

            return new BankValidationResponse
            {
                Success = false,
                Authorized = false
            };
        }

    }
}
