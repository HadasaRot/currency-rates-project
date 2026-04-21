using CurrencyRates.Data;
using CurrencyRates.Models;

namespace CurrencyRates.Services
{
    public class CurrencyDataService
    {
        private readonly AppDbContext _context;

        public CurrencyDataService(AppDbContext context)
        {
            _context = context;
        }

        public List<CurrencyRate> GetWeek()
        {
            var today = DateTime.Today;
            var fromDate = today.AddDays(-6);
            var data = _context.CurrencyRates
                .Where(x => x.RateDate >= fromDate && x.RateDate <= today)
                .ToList();
            return BuildTimeSeries(data, fromDate, today, days: 7);
        }

        public List<CurrencyRate> GetMonth()
        {
            var today = DateTime.Today;
            var fromDate = today.AddMonths(-1).AddDays(1);

            var dataFromDate = fromDate.AddDays(-7);


            var data = _context.CurrencyRates
                .Where(x => x.RateDate >= dataFromDate && x.RateDate <= today)
                .ToList();

            return BuildTimeSeries(data, fromDate, today);
        }

        public List<CurrencyRate> GetHalfYear()
        {
            var today = DateTime.Today;
            var fromDate = today.AddMonths(-6);
            var data = _context.CurrencyRates
                .Where(x => x.RateDate >= fromDate && x.RateDate <= today)
                .ToList();

            var result = new List<CurrencyRate>();
            var currencies = data.Select(x => x.CurrencyCode).Distinct();

            foreach (var currency in currencies)
            {
                for (int i = 5; i >= 0; i--)
                {
                    var targetDate = today.AddMonths(-i);
                    var record = data
                        .Where(x => x.CurrencyCode == currency && x.RateDate <= targetDate)
                        .OrderByDescending(x => x.RateDate)
                        .FirstOrDefault();

                    if (record != null)
                        result.Add(new CurrencyRate
                        {
                            CurrencyCode = currency,
                            RateDate = targetDate,
                            Rate = record.Rate
                        });
                }
            }

            return result.OrderBy(x => x.CurrencyCode).ThenBy(x => x.RateDate).ToList();
        }

        public List<CurrencyRate> GetYear()
        {
            var today = DateTime.Today;
            var data = _context.CurrencyRates.ToList();
            var result = new List<CurrencyRate>();
            var currencies = data.Select(x => x.CurrencyCode).Distinct();

            foreach (var currency in currencies)
            {
                for (int i = 0; i < 12; i++)
                {
                    var targetDate = today.AddMonths(-i);
                    var record = data
                        .Where(x => x.CurrencyCode == currency && x.RateDate <= targetDate)
                        .OrderByDescending(x => x.RateDate)
                        .FirstOrDefault();

                    if (record != null)
                        result.Add(new CurrencyRate
                        {
                            CurrencyCode = currency,
                            RateDate = targetDate,
                            Rate = record.Rate
                        });
                }
            }

            return result.OrderBy(x => x.CurrencyCode).ThenBy(x => x.RateDate).ToList();
        }

        private List<CurrencyRate> BuildTimeSeries(
            List<CurrencyRate> data,
            DateTime fromDate,
            DateTime toDate,
            int? days = null)
        {
            var result = new List<CurrencyRate>();
            var currencies = data.Select(x => x.CurrencyCode).Distinct();
            int range = days ?? (toDate - fromDate).Days + 1;

            foreach (var currency in currencies)
            {
                CurrencyRate lastKnown = data.Where(x => x.CurrencyCode == currency && x.RateDate.Date < fromDate.Date)
                                             .OrderByDescending(x => x.RateDate)
                                             .FirstOrDefault();
                for (int i = range - 1; i >= 0; i--)
                {
                    var date = toDate.AddDays(-i);
                    var record = data.FirstOrDefault(x =>
                        x.CurrencyCode == currency && x.RateDate.Date == date.Date);

                    if (record != null) lastKnown = record;

                    if (lastKnown != null)
                        result.Add(new CurrencyRate
                        {
                            CurrencyCode = currency,
                            RateDate = date,
                            Rate = lastKnown.Rate
                        });
                }
            }

            return result;
        }
    }
}