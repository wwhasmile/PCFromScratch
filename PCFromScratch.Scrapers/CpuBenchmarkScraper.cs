using System.Text.RegularExpressions;
using AngleSharp;
using PCFromScratch.DBModels;

namespace PCFromScratch.Scrapers;

public class CpuBenchmarkScraper
{
    private static readonly string[] Urls =
    {
        "https://www.cpubenchmark.net/cpu-list/intel",
        "https://www.cpubenchmark.net/cpu-list/amd"
    };

    public static async Task<List<CpuBenchmark>> ScrapeCpuBenchmarks()
    {
        var allCpus = new List<CpuBenchmark>();
        
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
                
                var cpuData = new List<CpuBenchmark>();

                //Look for the standard PassMark table format
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
                            var scoreStr = cols[1].TextContent.Trim();
                            int.TryParse(Regex.Replace(scoreStr, "[^0-9]", ""), out var score);

                            if (!string.IsNullOrEmpty(cpuName) && cpuName != "CPU Name")
                            {
                                cpuData.Add(new CpuBenchmark { Id = Guid.NewGuid(), Name = cpuName, Score = score });
                            }
                        }
                    }
                }
                else
                {
                    //Alternative list structure
                    var listItems = document.QuerySelectorAll("li");
                    foreach (var item in listItems)
                    {
                        var nameTag = item.QuerySelector("span.prdname");
                        var scoreTag = item.QuerySelector("span.count");
                        if (scoreTag == null) continue;
                        int.TryParse(Regex.Replace(scoreTag.TextContent.Trim(), "[^0-9]", ""), out var score);
                        if (nameTag != null)
                        {
                            cpuData.Add(new CpuBenchmark 
                            { 
                                Id = Guid.NewGuid(),
                                Name = nameTag.TextContent.Trim(), 
                                Score = score
                            });
                        }
                    }
                }

                if (!cpuData.Any())
                {
                    Console.WriteLine($"Warning: Could not find CPU data on {url}. The HTML structure might have changed.");
                }

                allCpus.AddRange(cpuData);
                
                //Wait between requests so we will not get blocked
                await Task.Delay(2000);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching {url}: {e.Message}");
            }
        }
        return allCpus;
    }
}