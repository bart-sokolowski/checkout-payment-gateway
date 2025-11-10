using System.Globalization;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Helpers
{
    public class PaymentValidator
    {
        public static ValidationResult<PostPaymentRequest> Validate(PostPaymentRequest request)
        {
            var errors = new List<string>();
            var currentYear = DateTime.UtcNow.Year;
            var currentMonth = DateTime.UtcNow.Month;

            //Validate payment details

            ////Card Number
            ///-Between 14-19 characters long
            ///-Must only contain numeric characters
            var cardNumber = request.CardNumber.Trim();
            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                errors.Add("Card number is required.");
            }
            else
            {
                if (cardNumber.Length < 14 || cardNumber.Length > 19)
                    errors.Add("Card number must be between 14 and 19 digits.");
                if (!cardNumber.All(char.IsDigit))
                    errors.Add("Card number must contain only numeric characters");
            }

            ////Expiry Month
            ///-Value must be between 1-12
            if (request.ExpiryMonth < 1 || request.ExpiryMonth > 12)
            {
                errors.Add("Expiry month must be between 1 and 12");
            }

            ////Expiry Year
            ///-Value must be in the future
            if ((request.ExpiryYear < currentYear) || (request.ExpiryYear == currentYear && request.ExpiryMonth < currentMonth))
            {
                errors.Add("Expiry date is invalid.");
            }

            ////Currency
            ///-Must be 3 characters
            ///-*Check if the currency is supported
            if(Consts.Consts.CURRENCIES.FirstOrDefault(c => c.IsoCode.ToUpper() == request.Currency.ToUpper()) == null)
            {
                errors.Add("Unsupported currency");
            }

            ////Amount
            ///-Must be an integer
            if (request.Amount <= 0)
            {
                errors.Add("Amount must be a positive integer");
            }

            ////CVV
            ///-Must be 3-4 characters long
            ///-Must only contain numeric characters
            var cvvStr = request.Cvv;

            if (!cvvStr.All(char.IsDigit))
            {
                errors.Add("CVV must contain only numeric characters.");
            }
            else
            {
                if (cvvStr.Length < 3 || cvvStr.Length > 4)
                {
                    errors.Add("CVV must be 3 or 4 digits.");
                }
            }

            if (errors.Any())
            {
                // reject payment due to validation errors
                return ValidationResult<PostPaymentRequest>.Fail(errors, request);
            }

            return ValidationResult<PostPaymentRequest>.Success(request);
        }
    }
}
