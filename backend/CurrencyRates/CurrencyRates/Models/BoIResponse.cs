namespace CurrencyRates.Models
{
    public class BoIResponse
    {
        public List<BoIRate> exchangeRates { get; set; }
    }
    public class BoIRate
    {
        public string key { get; set; }
        public decimal currentExchangeRate { get; set; }
        public DateTime lastUpdate { get; set; }
    }
}


