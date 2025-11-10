using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Consts
{
    public static class Consts
    {
        public static readonly string DEFAULT_CURRENCY = "GBP";

        //List of base currencies, list is not full, for the purposes of the demo
        public static readonly List<Currency> CURRENCIES = new()
        {
            new Currency("US Dollar", "USD"),
            new Currency("Euro", "EUR"),
            new Currency("British Pound", "GBP")
        };
    }
}
