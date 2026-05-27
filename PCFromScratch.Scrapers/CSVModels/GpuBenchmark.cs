using CsvHelper.Configuration.Attributes;

namespace PCFromScratch.Scrapers.CSVModels;

public class GpuBenchmark
{
    [Name("GPU Name")]
    public string? GpuName { get; set; }

    [Name("Benchmark Score")]
    public string? BenchmarkScore { get; set; }
}
