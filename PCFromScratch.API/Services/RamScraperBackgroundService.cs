using PCFromScratch.Repository;
using PCFromScratch.Scrapers;

namespace PCFromScratch.API.Services;

public class RamScraperBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<RamScraperBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private readonly ILogger<RamScraperBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started Ram Scraper background task.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Ram Scraping started at: {time}.", DateTimeOffset.Now);

            try
            {
                await ScrapeRams();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing Ram Scraper work.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task ScrapeRams()
    {
        var rams = await RamScraper.GetRams();

        using var scope = _serviceScopeFactory.CreateScope();
        var ramRepository = scope.ServiceProvider.GetRequiredService<IRamRepository>();

        foreach (var ram in rams)
            await ramRepository.AddRam(ram);
    }
}