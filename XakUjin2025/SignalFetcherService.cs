using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace XakUjin2025
{
    public class SignalFetcherService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpClient _httpClient = new();

        public SignalFetcherService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int[] apartmentIds = { 2118, 2119, 2120, 2121, 2122 };
            List<string> egPool = new()
            {
                "490ae3a8-1a8f-426f-b21b-04fe8e8483de",
                "6f843503-5dc6-4222-a111-aaab4b97e39d",
                "06ed9754-0be8-4d04-be8e-69689a888ca8",
                "c54855b4-ee02-46fc-8b4e-8a90804ef2b5",
                "2fc4f90a-346d-4d1c-aebc-6ff45a9c3136"
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                var foundAddresses = new HashSet<string>();
                var foundApartmentIds = new HashSet<int>();

                for (int i = 0; i < apartmentIds.Length; i++)
                {
                    int apartmentId = apartmentIds[i];
                    string eg = egPool[i % egPool.Count];

                    string url = $"https://api-uae-test.ujin.tech/api/huk/get-device-signals/?" +
                                 $"apartment_id={apartmentId}&eg={eg}&egt=6&token=con-10892-057f7948807f893374d5b4c8bdf397d3";

                    try
                    {
                        var response = await _httpClient.GetAsync(url, stoppingToken);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync(stoppingToken);
                            var data = JsonConvert.DeserializeObject<GetDeviceSignalsResponseModel>(json);

                            if (data?.Data?.Apartment?.ApartmentTitle is string fullAddress && data.Error == 0)
                            {
                                string cleanAddress = Regex.Replace(fullAddress.Trim(), @"\s*,\s*кв\..*$", "").Trim();

                                if (!string.IsNullOrEmpty(cleanAddress))
                                {
                                    foundAddresses.Add(cleanAddress);
                                    foundApartmentIds.Add(apartmentId);

                                    using var scope = _serviceProvider.CreateScope();
                                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                                    var home = await db.Homes.FirstOrDefaultAsync(h => h.Address == cleanAddress, stoppingToken);
                                    if (home == null)
                                    {
                                        db.Homes.Add(new Home { Address = cleanAddress, IsDelete = false });
                                        await db.SaveChangesAsync(stoppingToken);
                                    }
                                    else if (home.IsDelete)
                                    {
                                        home.IsDelete = false;
                                        await db.SaveChangesAsync(stoppingToken);
                                    }

                                    var apartment = await db.Apartments.FirstOrDefaultAsync(a => a.ApartmentId == apartmentId, stoppingToken);
                                    if (apartment == null)
                                    {
                                        db.Apartments.Add(new Apartment
                                        {
                                            ApartmentId = apartmentId,
                                            ApartmentTitle = data.Data.Apartment.ApartmentTitle,
                                            HomeId = home.IdHome,
                                            IsDelete = false
                                        });
                                        await db.SaveChangesAsync(stoppingToken);
                                    }
                                    else
                                    {
                                        apartment.ApartmentTitle = data.Data.Apartment.ApartmentTitle;
                                        apartment.HomeId = home.IdHome;

                                        if (apartment.IsDelete)
                                            apartment.IsDelete = false;

                                        await db.SaveChangesAsync(stoppingToken);
                                    }

                                    if (data.Data.Signals != null)
                                    {
                                        if (!apartment.IsDelete)
                                        {
                                            var signalsExistDb = await db.Signals
                                                .Where(s => s.ApartmentId == apartment.ApartmentId)
                                                .ToListAsync(stoppingToken);

                                            foreach (var signal in signalsExistDb)
                                            {
                                                if (data.Data.Signals.ContainsKey(signal.SignalSN))
                                                {
                                                    signal.IsDelete = false;
                                                    db.Signals.Update(signal);
                                                    if (data.Data.Signals.TryGetValue(signal.SignalSN, out var indicationsFromJson))
                                                    {
                                                        foreach (var indicationJson in indicationsFromJson)
                                                        {
                                                            var newIndication = new Indication
                                                            {
                                                                SignalId = signal.SignalId,
                                                                IndicationName = indicationJson.Name,
                                                                IndicationLabel = indicationJson.SignalLabel,
                                                                IndicationValue = indicationJson.Intensity,
                                                                CreatedAt = DateTime.Now
                                                            };

                                                            db.Indications.Add(newIndication);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    signal.IsDelete = true;
                                                    db.Signals.Update(signal);
                                                }
                                            }

                                            foreach (var kvp in data.Data.Signals)
                                            {
                                                string sn = kvp.Key;
                                                var signalItems = kvp.Value;

                                                var signalExist = await db.Signals
                                                    .FirstOrDefaultAsync(s => s.SignalSN == sn && s.ApartmentId == apartment.ApartmentId, stoppingToken);

                                                if (signalExist == null)
                                                {
                                                    var newSignal = new Signal
                                                    {
                                                        ApartmentId = apartment.ApartmentId,
                                                        SignalSN = sn,
                                                        IsDelete = false
                                                    };
                                                    db.Signals.Add(newSignal);
                                                    await db.SaveChangesAsync(stoppingToken);

                                                    if (data.Data.Signals.TryGetValue(newSignal.SignalSN, out var indicationsFromJson))
                                                    {
                                                        foreach (var indicationJson in indicationsFromJson)
                                                        {
                                                            var newIndication = new Indication
                                                            {
                                                                SignalId = newSignal.SignalId,
                                                                IndicationName = indicationJson.Name,
                                                                IndicationLabel = indicationJson.SignalLabel,
                                                                IndicationValue = indicationJson.Intensity,
                                                                CreatedAt = DateTime.Now
                                                            };

                                                            db.Indications.Add(newIndication);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    signalExist.IsDelete = false;
                                                    db.Signals.Update(signalExist);

                                                    if (data.Data.Signals.TryGetValue(signalExist.SignalSN, out var indicationsFromJson))
                                                    {
                                                        foreach (var indicationJson in indicationsFromJson)
                                                        {
                                                            var newIndication = new Indication
                                                            {
                                                                SignalId = signalExist.SignalId,
                                                                IndicationName = indicationJson.Name,
                                                                IndicationLabel = indicationJson.SignalLabel,
                                                                IndicationValue = indicationJson.Intensity,
                                                                CreatedAt = DateTime.Now
                                                            };

                                                            db.Indications.Add(newIndication);
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        await db.SaveChangesAsync(stoppingToken);
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }

                // Обновление всех домов после прохода по apartmentIds
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var allHomes = await db.Homes.ToListAsync(stoppingToken);
                    var allApts = await db.Apartments.ToListAsync(stoppingToken);

                    foreach (var home in allHomes)
                    {
                        home.IsDelete = !foundAddresses.Contains(home.Address);
                    }

                    foreach (var apt in allApts)
                    {
                        apt.IsDelete = !foundApartmentIds.Contains(apt.ApartmentId);
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(300), stoppingToken);
            }
        }

    }

}
