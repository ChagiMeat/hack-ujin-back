using Microsoft.EntityFrameworkCore;

namespace XakUjin2025
{
    public class DeviceInfoFetcherService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpClient _httpClient = new();

        public DeviceInfoFetcherService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<string> egPool = new()
            {
                "490ae3a8-1a8f-426f-b21b-04fe8e8483de",
                "6f843503-5dc6-4222-a111-aaab4b97e39d",
                "06ed9754-0be8-4d04-be8e-69689a888ca8",
                "c54855b4-ee02-46fc-8b4e-8a90804ef2b5",
                "2fc4f90a-346d-4d1c-aebc-6ff45a9c3136"
            };

            int i = 0;
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var allSignals = await db.Signals.ToListAsync();
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var signal in allSignals)
                {
                    if (signal.IsDelete)
                        continue;
                    string url = $"https://api-uae-test.ujin.tech/api/huk/get-device-info/?" +
                                 $"serialnumber={signal.SignalSN}&eg={egPool[i]}&egt=6&token=con-10892-057f7948807f893374d5b4c8bdf397d3";
                    try
                    {
                        var response = await _httpClient.GetAsync(url, stoppingToken);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync(stoppingToken);
                            Console.WriteLine("RAW JSON:" + json);
                            Console.WriteLine("URL: " + url);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
