using PCFromScratch.Repository;
using PCFromScratch.Scrapers;

namespace PCFromScratch.API.Services;

public class CpuBenchmarkScraperBackgroundService (IServiceScopeFactory serviceScopeFactory, ILogger<CpuBenchmarkScraperBackgroundService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private readonly ILogger<CpuBenchmarkScraperBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started Cpu Benchmark Scraper background task.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cpu Benchmark Scraping started at: {time}.", DateTimeOffset.Now);

            try
            {
                await ScrapeCpuBenchmarks();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing Cpu Benchmark Scraper work.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task ScrapeCpuBenchmarks()
    {
        var benchmarks = await CpuBenchmarkScraper.ScrapeCpuBenchmarks();

        using var scope = _serviceScopeFactory.CreateScope();
        var cpuBenchmarkRepository = scope.ServiceProvider.GetRequiredService<ICpuBenchmarkRepository>();

        foreach (var benchmark in benchmarks)
            await cpuBenchmarkRepository.AddCpuBenchmark(benchmark);
    }
}