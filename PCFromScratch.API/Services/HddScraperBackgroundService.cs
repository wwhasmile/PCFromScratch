using PCFromScratch.Repository;
using PCFromScratch.Scrapers;

namespace PCFromScratch.API.Services;

public class HddScraperBackgroundService(IServiceScopeFactory serviceScopeFactory,
        ILogger<HddScraperBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private readonly ILogger<HddScraperBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started HDD Scraper background task.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("HDD Scraping started at: {time}.", DateTimeOffset.Now);

            try
            {
                await ScrapeHdds();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing HDD Scraper work.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task ScrapeHdds()
    {
        var hdds = await HddScraper.GetHdds();

        using var scope = _serviceScopeFactory.CreateScope();
        var internalDriveRepository = scope.ServiceProvider.GetRequiredService<IInternalDriveRepository>();

        foreach (var hdd in hdds)
            await internalDriveRepository.AddInternalDrive(hdd);
    }
}