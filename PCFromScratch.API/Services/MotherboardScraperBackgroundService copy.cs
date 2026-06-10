using PCFromScratch.Repository;
using PCFromScratch.Scrapers;

namespace PCFromScratch.API.Services;

public class MotherboardScraperBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<MotherboardScraperBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private readonly ILogger<MotherboardScraperBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started Motherboard Scraper background task.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Motherboard Scraping started at: {time}.", DateTimeOffset.Now);

            try
            {
                await ScrapeMotherboards();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing Motherboard Scraper work.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task ScrapeMotherboards()
    {
        var motherboards = await MotherboardScraper.GetMotherboards();

        using var scope = _serviceScopeFactory.CreateScope();
        var motherboardRepository = scope.ServiceProvider.GetRequiredService<IMotherboardRepository>();

        foreach (var motherboard in motherboards)
            await motherboardRepository.AddMotherboard(motherboard);
    }
}