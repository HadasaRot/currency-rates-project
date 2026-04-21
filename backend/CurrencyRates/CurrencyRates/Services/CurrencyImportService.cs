using CurrencyRates.Data;
using CurrencyRates.Models;
using System.Globalization;

namespace CurrencyRates.Services
{
    public class CurrencyImportService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public CurrencyImportService(HttpClient httpClient, AppDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }


        private string MapName(string key)
        {
            return key switch
            {
                "USD" => "דולר",
                "GBP" => "לירה",
                "CHF" => "פרנק",
                "SEK" => "כתר",
                _ => key
            };
        }
        public async Task<string> FetchAndSaveRates()
        {
            var start = DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd");
            var end = DateTime.Today.ToString("yyyy-MM-dd");
            var url =
                   $"https://edge.boi.gov.il/FusionEdgeServer/sdmx/v2/data/dataflow/BOI.STATISTICS/EXR/1.0/" +
                   $"RER_USD_ILS,RER_GBP_ILS,RER_SEK_ILS,RER_CHF_ILS" +
                   $"?c%5BDATA_TYPE%5D=OF00&startperiod={start}&endperiod={end}&format=csv";

           var csv = await _httpClient.GetStringAsync(url);
            var lines = csv.Split('\n');

            int saved = 0;
            int skipped = 0;
            int errors = 0;

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var cols = line.Split(',');

                if (cols.Length < 14) { errors++; continue; }

                try
                {
                    var currencyCode = cols[0].Split('_')[1];
                    var date = DateTime.Parse(cols[12]);
                    var rate = decimal.Parse(cols[13], CultureInfo.InvariantCulture);

                    var exists = _context.CurrencyRates.Any(x =>
                        x.CurrencyCode == currencyCode && x.RateDate == date);

                    if (exists) { skipped++; continue; }

                    _context.CurrencyRates.Add(new CurrencyRate
                    {
                        CurrencyCode = currencyCode,
                        CurrencyName = MapName(currencyCode),
                        Rate = rate,
                        RateDate = date
                    });
                    saved++;
                }
                catch (Exception ex)
                {
                    errors++;
                }
            }

            await _context.SaveChangesAsync();
            return $"saved={saved}, skipped={skipped}, errors={errors}, totalLines={lines.Length}";
        }
    }
}