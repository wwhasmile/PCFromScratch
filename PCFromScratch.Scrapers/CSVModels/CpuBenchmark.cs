using CsvHelper.Configuration.Attributes;

namespace PCFromScratch.Scrapers.CSVModels;

public class CpuBenchmark
{
    [Name("CPU Name")]
    public string? CpuName { get; set; }

    [Name("Benchmark Score")]
    public string? BenchmarkScore { get; set; }
}
