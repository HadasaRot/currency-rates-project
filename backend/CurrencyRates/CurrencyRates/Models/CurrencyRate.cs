namespace CurrencyRates.Models
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public decimal Rate { get; set; }
        public DateTime RateDate { get; set; }
    }
}