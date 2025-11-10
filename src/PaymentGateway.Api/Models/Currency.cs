namespace PaymentGateway.Api.Models
{
    public class Currency
    {
        public string Name { get; set; }
        public string IsoCode { get; set; }

        public Currency(string name, string isoCode)
        {
            Name = name;
            IsoCode = isoCode.ToUpperInvariant();
        }
    }
}
