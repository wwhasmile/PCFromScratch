using System.Text.RegularExpressions;

using AngleSharp;

using PCFromScratch.DBModels;

namespace PCFromScratch.Scrapers;

public class GpuBenchmarkScraper
{
    private const string Url = "https://www.videocardbenchmark.net/gpu_list.php";

    public static async Task<List<GpuBenchmark>> ScrapeGpuBenchmarks()
    {
        Console.WriteLine($"Fetching data from {Url}...");
        
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

        string htmlContent;
        try
        {
            var response = await httpClient.GetAsync(Url);
            response.EnsureSuccessStatusCode();
            htmlContent = await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching URL: {e.Message}");
            return new List<GpuBenchmark>();
        }

        Console.WriteLine("Parsing HTML with AngleSharp...");
        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(htmlContent));

        var table = document.QuerySelector("table#cputable");
        if (table == null)
        {
            Console.WriteLine("Error: Could not find the GPU table with id='cputable'.");
            return new List<GpuBenchmark>();
        }

        var gpuData = new List<GpuBenchmark>();
        var tbody = table.QuerySelector("tbody");
        var rows = tbody != null ? tbody.QuerySelectorAll("tr") : table.QuerySelectorAll("tr");

        foreach (var row in rows)
        {
            var cols = row.QuerySelectorAll("th, td");
            if (cols.Length >= 2)
            {
                var gpuName = cols[0].TextContent.Trim();
                var scoreStr = cols[1].TextContent.Trim();
                int.TryParse(Regex.Replace(scoreStr, "[^0-9]", ""), out var score);
                gpuData.Add(new GpuBenchmark { Id = Guid.NewGuid(), Name = gpuName, Score = score });
            }
        }

        var headersToFilter = new HashSet<string> { "Videocard Name", "GPU Name", "Video Card Name" };
        var cleanedData = gpuData
            .Where(g => !headersToFilter.Contains(g.Name))
            .Where(g => !string.IsNullOrEmpty(g.Name) && g.Score != 0)
            .ToList();
        return cleanedData;
    }
}
