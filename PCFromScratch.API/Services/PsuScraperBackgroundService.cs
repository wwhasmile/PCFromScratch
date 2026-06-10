using PCFromScratch.Repository;
using PCFromScratch.Scrapers;

namespace PCFromScratch.API.Services;

public class PsuScraperBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<PsuScraperBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private readonly ILogger<PsuScraperBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started Psu Scraper background task.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Psu Scraping started at: {time}.", DateTimeOffset.Now);

            try
            {
                await ScrapePsus();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing Psu Scraper work.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task ScrapePsus()
    {
        var psus = await PsuScraper.GetPsus();

        using var scope = _serviceScopeFactory.CreateScope();
        var psuRepository = scope.ServiceProvider.GetRequiredService<IPsuRepository>();

        foreach (var psu in psus)
            await psuRepository.AddPsu(psu);
    }
}