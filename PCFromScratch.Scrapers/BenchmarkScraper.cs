using System.Globalization;
using AngleSharp;
using CsvHelper;

namespace PCFromScratch.Scrapers;

public class BenchmarkScraper
{
    private static readonly string[] Urls =
    {
        "https://www.cpubenchmark.net/cpu-list/intel",
        "https://www.cpubenchmark.net/cpu-list/amd"
    };

    private const string FilePath = "data/cpu_benchmarks.csv";

    public static async Task ScrapeCpuBenchmarks()
    {
        if (!File.Exists(FilePath))
        {
            string? directoryName = Path.GetDirectoryName(FilePath);
            if (directoryName is not null && !Directory.Exists(Path.GetDirectoryName(FilePath)))
            {
                Directory.CreateDirectory(directoryName);
            }
            File.Create(FilePath).Close();
        }
        
        var allCpus = new List<CSVModels.CpuBenchmark>();
        
        // Setup HttpClient with headers to mimic a real browser
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
        
        // Setup AngleSharp context
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);

        foreach (var url in Urls)
        {
            Console.WriteLine($"Scraping {url}...");
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var htmlContent = await response.Content.ReadAsStringAsync();

                var document = await context.OpenAsync(req => req.Content(htmlContent));
                
                var cpuData = new List<CSVModels.CpuBenchmark>();

                // Attempt 1: Look for the standard PassMark table format
                var table = document.QuerySelector("table#cputable");
                if (table != null)
                {
                    var tbody = table.QuerySelector("tbody");
                    var rows = tbody != null ? tbody.QuerySelectorAll("tr") : table.QuerySelectorAll("tr");

                    foreach (var row in rows)
                    {
                        var cols = row.QuerySelectorAll("td");
                        if (cols.Length >= 2)
                        {
                            var cpuName = cols[0].TextContent.Trim();
                            var score = cols[1].TextContent.Trim();

                            if (!string.IsNullOrEmpty(cpuName) && !string.IsNullOrEmpty(score) && cpuName != "CPU Name")
                            {
                                cpuData.Add(new CSVModels.CpuBenchmark { CpuName = cpuName, BenchmarkScore = score });
                            }
                        }
                    }
                }
                else
                {
                    // Attempt 2: Alternative list structure
                    var listItems = document.QuerySelectorAll("li");
                    foreach (var item in listItems)
                    {
                        var nameTag = item.QuerySelector("span.prdname");
                        var scoreTag = item.QuerySelector("span.count");

                        if (nameTag != null && scoreTag != null)
                        {
                            cpuData.Add(new CSVModels.CpuBenchmark 
                            { 
                                CpuName = nameTag.TextContent.Trim(), 
                                BenchmarkScore = scoreTag.TextContent.Trim() 
                            });
                        }
                    }
                }

                if (!cpuData.Any())
                {
                    Console.WriteLine($"Warning: Could not find CPU data on {url}. The HTML structure might have changed.");
                }

                allCpus.AddRange(cpuData);
                
                // Be polite to the server and wait a second between requests
                await Task.Delay(2000);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching {url}: {e.Message}");
            }
        }
        
        // Ensure data directory exists
        var dataDir = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrEmpty(dataDir) && !Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }

        using (var writer = new StreamWriter(FilePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(allCpus);
        }
        
        Console.WriteLine($"\nSuccessfully saved {allCpus.Count} CPUs to '{FilePath}'.");
    }
}