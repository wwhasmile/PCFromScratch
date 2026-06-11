using PCFromScratch.Repository;
using PCFromScratch.Scrapers;

namespace PCFromScratch.API.Services;

public class GpuBenchmarkScraperBackgroundService (IServiceScopeFactory serviceScopeFactory, ILogger<GpuBenchmarkScraperBackgroundService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private readonly ILogger<GpuBenchmarkScraperBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started Gpu Benchmark Scraper background task.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Gpu Benchmark Scraping started at: {time}.", DateTimeOffset.Now);

            try
            {
                await ScrapeGpuBenchmarks();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing Gpu Benchmark Scraper work.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task ScrapeGpuBenchmarks()
    {
        var benchmarks = await GpuBenchmarkScraper.ScrapeGpuBenchmarks();

        using var scope = _serviceScopeFactory.CreateScope();
        var gpuBenchmarkRepository = scope.ServiceProvider.GetRequiredService<IGpuBenchmarkRepository>();

        foreach (var benchmark in benchmarks)
            await gpuBenchmarkRepository.AddGpuBenchmark(benchmark);
    }
}