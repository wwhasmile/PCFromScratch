using PCFromScratch.Repository;
using PCFromScratch.Scrapers;

namespace PCFromScratch.API.Services;

public class CpuScraperBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<CpuScraperBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private readonly ILogger<CpuScraperBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started Cpu Scraper background task.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cpu Scraping started at: {time}.", DateTimeOffset.Now);

            try
            {
                await ScrapeCpus();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing Cpu Scraper work.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task ScrapeCpus()
    {
        var cpus = await CpuScraper.GetCpus();

        using var scope = _serviceScopeFactory.CreateScope();
        var cpuRepository = scope.ServiceProvider.GetRequiredService<ICpuRepository>();

        foreach (var cpu in cpus)
            await cpuRepository.AddCpu(cpu);
    }
}