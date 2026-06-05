using CsvHelper.Configuration.Attributes;

namespace PCFromScratch.Scrapers.CSVModels;

public class MatchedGpu
{
    [Name("Shop Name")]
    public string? ShopName { get; set; }

    [Name("Benchmark Name")]
    public string? MatchedBenchmarkName { get; set; }

    [Name("Benchmark Score")]
    public string? BenchmarkScore { get; set; }

    [Name("Match Confidence")]
    public int MatchConfidence { get; set; }

    [Name("Shop Link")]
    public string? ShopLink { get; set; }

    [Name("TDP")]
    public int Tdp { get; set; }
}
