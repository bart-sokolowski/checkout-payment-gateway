namespace PaymentGateway.Api.Models
{
    public class BankValidationResponse
    {
        public bool Success { get; set; }
        public bool Authorized { get; set; }
        public string? AuthorizationCode { get; set; }
    }
}
