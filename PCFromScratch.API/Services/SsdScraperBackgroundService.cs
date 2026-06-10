using PCFromScratch.Repository;
using PCFromScratch.Scrapers;

namespace PCFromScratch.API.Services;

public class SsdScraperBackgroundService(IServiceScopeFactory serviceScopeFactory,
        ILogger<SsdScraperBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private readonly ILogger<SsdScraperBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started SSD Scraper background task.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("SSD Scraping started at: {time}.", DateTimeOffset.Now);

            try
            {
                await ScrapeSsds();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing SSD Scraper work.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task ScrapeSsds()
    {
        var ssds = await SsdScraper.GetSsds();

        using var scope = _serviceScopeFactory.CreateScope();
        var internalDriveRepository = scope.ServiceProvider.GetRequiredService<IInternalDriveRepository>();

        foreach (var ssd in ssds)
            await internalDriveRepository.AddInternalDrive(ssd);
    }
}