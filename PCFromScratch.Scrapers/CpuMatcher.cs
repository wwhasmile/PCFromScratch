using System.Globalization;
using CsvHelper;
using FuzzySharp;
using PCFromScratch.Scrapers.CSVModels;

namespace PCFromScratch.Scrapers;

public class CpuMatcher
{
    //TODO switch all methods to work with db instead of csv files
    public static void MatchCpusWithBenchmarks(string shopCsvPath, string benchmarkCsvPath, string outputCsvPath)
    {
        Console.WriteLine("Loading shop data...");
        var shopCpus = LoadShopCpus(shopCsvPath);
        
        Console.WriteLine("Loading benchmark data...");
        var benchmarks = LoadBenchmarks(benchmarkCsvPath);
        var benchmarkNames = benchmarks.Select(b => b.CpuName).ToList();

        var matchedData = new List<MatchedCpu>();

        Console.WriteLine("Matching CPUs (this might take a moment)...");
        foreach (var shopCpu in shopCpus)
        {
            if (string.IsNullOrWhiteSpace(shopCpu.Name)) continue;

            // We use Process.ExtractOne which returns the best match from the list
            // We pass in the shop name and the list of all benchmark names
            var result = Process.ExtractOne(shopCpu.Name, benchmarkNames);

            var matched = new MatchedCpu
            {
                ShopName = shopCpu.Name,
                ShopLink = shopCpu.Link,
                Socket = shopCpu.Socket,
                Tdp = shopCpu.Tdp,
                RamMax = shopCpu.RamMax,
                RamGen = shopCpu.RamGen,
                RamFreq = shopCpu.RamFreq,
                BoxOem = shopCpu.BoxOem
            };

            // You can adjust this threshold (0-100). 
            // 70 is generally a safe bet for slight variations in names.
            if (result != null && result.Score >= 70)
            {
                var bestBenchmark = benchmarks.First(b => b.CpuName == result.Value);
                matched.MatchedBenchmarkName = bestBenchmark.CpuName;
                matched.BenchmarkScore = bestBenchmark.BenchmarkScore;
                matched.MatchConfidence = result.Score;
                Console.WriteLine($"[✓] {shopCpu.Name}  ==>  {bestBenchmark.CpuName} (Score: {result.Score})");
            }
            else
            {
                matched.MatchedBenchmarkName = "NO MATCH FOUND";
                matched.BenchmarkScore = "";
                matched.MatchConfidence = result?.Score ?? 0;
                Console.WriteLine($"[✗] {shopCpu.Name}  ==>  No suitable match found (Best score: {result?.Score ?? 0})");
            }

            matchedData.Add(matched);
        }

        Console.WriteLine($"\nWriting results to {outputCsvPath}...");
        using (var writer = new StreamWriter(outputCsvPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(matchedData);
        }
        
        Console.WriteLine("Matching complete!");
    }

    private static List<Cpu> LoadShopCpus(string path)
    {
        if (!File.Exists(path)) return new List<Cpu>();
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<Cpu>().ToList();
    }

    private static List<CpuBenchmark> LoadBenchmarks(string path)
    {
        if (!File.Exists(path)) return new List<CpuBenchmark>();
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<CpuBenchmark>().ToList();
    }

    public static CpuBenchmark GetBenchmark(string nameInStore)
    {
        var benchmarks = LoadBenchmarks("data/cpu_benchmarks.csv");
        var benchmarkNames = benchmarks.Select(b => b.CpuName).ToList();
        var result = Process.ExtractOne(nameInStore, benchmarkNames);
        return benchmarks.First(b => b.CpuName == result.Value);
    }
}
