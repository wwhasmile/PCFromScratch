using System.Globalization;
using CsvHelper;
using FuzzySharp;
using PCFromScratch.Scrapers.CSVModels;

namespace PCFromScratch.Scrapers;

public class GpuMatcher
{
    public static void MatchGpusWithBenchmarks(string shopCsvPath, string benchmarkCsvPath, string outputCsvPath)
    {
        Console.WriteLine("Loading shop data...");
        var shopGpus = LoadShopGpus(shopCsvPath);
        
        Console.WriteLine("Loading benchmark data...");
        var benchmarks = LoadBenchmarks(benchmarkCsvPath);
        var benchmarkNames = benchmarks.Select(b => b.GpuName).ToList();

        var matchedData = new List<MatchedGpu>();

        Console.WriteLine("Matching GPUs (this might take a moment)...");
        foreach (var shopGpu in shopGpus)
        {
            if (string.IsNullOrWhiteSpace(shopGpu.Name)) continue;

            // We use Process.ExtractOne which returns the best match from the list
            var result = Process.ExtractOne(shopGpu.Name, benchmarkNames);

            var matched = new MatchedGpu
            {
                ShopName = shopGpu.Name,
                ShopLink = shopGpu.Link,
                Tdp = shopGpu.Tdp
            };

            // You can adjust this threshold (0-100). 
            // 70 is generally a safe bet, but GPUs might need tweaking if naming is very loose.
            if (result != null && result.Score >= 70)
            {
                var bestBenchmark = benchmarks.First(b => b.GpuName == result.Value);
                matched.MatchedBenchmarkName = bestBenchmark.GpuName;
                matched.BenchmarkScore = bestBenchmark.BenchmarkScore;
                matched.MatchConfidence = result.Score;
                Console.WriteLine($"[✓] {shopGpu.Name}  ==>  {bestBenchmark.GpuName} (Score: {result.Score})");
            }
            else
            {
                matched.MatchedBenchmarkName = "NO MATCH FOUND";
                matched.BenchmarkScore = "";
                matched.MatchConfidence = result?.Score ?? 0;
                Console.WriteLine($"[✗] {shopGpu.Name}  ==>  No suitable match found (Best score: {result?.Score ?? 0})");
            }

            matchedData.Add(matched);
        }

        Console.WriteLine($"\nWriting results to {outputCsvPath}...");
        using (var writer = new StreamWriter(outputCsvPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(matchedData);
        }
        
        Console.WriteLine("GPU Matching complete!");
    }

    private static List<Gpu> LoadShopGpus(string path)
    {
        if (!File.Exists(path)) return new List<Gpu>();
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<Gpu>().ToList();
    }

    private static List<GpuBenchmark> LoadBenchmarks(string path)
    {
        if (!File.Exists(path)) return new List<GpuBenchmark>();
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<GpuBenchmark>().ToList();
    }

    public static GpuBenchmark GetBenchmark(string nameInStore)
    {
        var benchmarks = LoadBenchmarks("data/gpu_benchmarks.csv");
        var benchmarkNames = benchmarks.Select(b => b.GpuName).ToList();
        var result = Process.ExtractOne(nameInStore, benchmarkNames);
        return benchmarks.First(b => b.GpuName == result.Value);
    }
}
