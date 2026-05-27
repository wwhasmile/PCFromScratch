using CsvHelper.Configuration.Attributes;

namespace PCFromScratch.Scrapers.CSVModels;
//Maybe 3 different models only for cpu in scrapers is not ok, but for now I only want it to work 
public class MatchedCpu
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

    [Name("Socket")]
    public string? Socket { get; set; }

    [Name("TDP")]
    public string? Tdp { get; set; }

    [Name("Max RAM")]
    public string? RamMax { get; set; }

    [Name("RAM Gen")]
    public string? RamGen { get; set; }

    [Name("RAM Freq")]
    public string? RamFreq { get; set; }

    [Name("Box/OEM")]
    public string? BoxOem { get; set; }
}
