using PCFromScratch.Repository;
using PCFromScratch.Scrapers;

namespace PCFromScratch.API.Services;

public class CoolerScraperBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<CoolerScraperBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private readonly ILogger<CoolerScraperBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started Cooler Scraper background task.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cooler Scraping started at: {time}.", DateTimeOffset.Now);

            try
            {
                await ScrapeCoolers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing Cooler Scraper work.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task ScrapeCoolers()
    {
        var coolers = await CoolerScraper.GetCoolers();

        using var scope = _serviceScopeFactory.CreateScope();
        var coolerRepository = scope.ServiceProvider.GetRequiredService<ICoolerRepository>();

        foreach (var cooler in coolers)
            await coolerRepository.AddCooler(cooler);
    }
}