using PCFromScratch.Repository;
using PCFromScratch.Scrapers;

namespace PCFromScratch.API.Services;

public class GpuScraperBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<GpuScraperBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private readonly ILogger<GpuScraperBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started Gpu Scraper background task.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Gpu Scraping started at: {time}.", DateTimeOffset.Now);

            try
            {
                await ScrapeGpus();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing Gpu Scraper work.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task ScrapeGpus()
    {
        var gpus = await GpuScraper.GetGpus();

        using var scope = _serviceScopeFactory.CreateScope();
        var gpuRepository = scope.ServiceProvider.GetRequiredService<IGpuRepository>();

        foreach (var gpu in gpus)
            await gpuRepository.AddGpu(gpu);
    }
}