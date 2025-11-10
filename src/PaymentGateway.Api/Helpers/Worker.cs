using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Helpers
{
    public class Worker
    {

        public static PostPaymentResponse MapPaymnetResponseModel(PostPaymentRequest request, Enums.PaymentStatus paymentStatus)
        {
            return new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = paymentStatus,
                CardNumberLastFour = GetLast4Digits(request.CardNumber),
                ExpiryMonth = request.ExpiryMonth,
                ExpiryYear = request.ExpiryYear,
                Currency = request.Currency.ToUpperInvariant(),
                Amount = request.Amount
            };

        }

        public static int GetLast4Digits(string cardNumber)
        {
            var last4 = cardNumber.Trim()[^4..];

            return int.Parse(last4);
        }
    }
}
