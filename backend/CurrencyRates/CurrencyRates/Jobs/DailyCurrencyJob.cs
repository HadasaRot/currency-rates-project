using Microsoft.Extensions.Hosting;
using CurrencyRates.Services;

namespace CurrencyRates.Jobs
{
    public class DailyCurrencyJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DailyCurrencyJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                var nextRun = DateTime.Today.AddHours(16);
                //var nextRun = DateTime.Now.AddMinutes(1);

                if (now > nextRun)
                    nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;

                await Task.Delay(delay, stoppingToken);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<CurrencyImportService>();

                    try
                    {
                        await service.FetchAndSaveRates();
                        Console.WriteLine("Daily currency job ran successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Job failed: " + ex.Message);
                    }
                }
            }
        }
    }
}