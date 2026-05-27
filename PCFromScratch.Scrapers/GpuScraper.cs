using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using CsvHelper;
using Microsoft.Playwright;

using PCFromScratch.Scrapers.CSVModels;

namespace PCFromScratch.Scrapers;

public class GpuScraper
{
    private static readonly string FilePath = "data/gpus.csv";
    public static async Task GetGpusForSale()
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
        
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false // Set to false to see the browser UI
        });
        var page = await browser.NewPageAsync();

        var gpus = new List<Gpu>();
        var random = new Random();

        var pageLink = "https://ek.ua/ua/list/189/";
        try
        {
            while (pageLink != null)
            {
                Console.WriteLine($"Retrieving page: {pageLink}");
                await page.GotoAsync(pageLink);

                // Simulate Human Behavior
                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight / 2);");
                await Task.Delay(random.Next(1000, 2500));
                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight);");
                await Task.Delay(random.Next(4500, 8500));

                var content = await page.ContentAsync();
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(content));

                var cards = document.QuerySelectorAll("td.model-short-info");

                foreach (var card in cards)
                {
                    try
                    {
                        var name = card.QuerySelector("span.u")?.TextContent;
                        if (string.IsNullOrEmpty(name)) continue;

                        var link = "https://ek.ua" + card.QuerySelector("a.model-short-title.no-u")?.GetAttribute("href");

                        var detailsDiv = card.QuerySelector("div.m-s-f2");
                        if (detailsDiv == null) continue;

                        var details = detailsDiv.ChildNodes;
                        string tdp = "";

                        foreach (var detail in details)
                        {
                            var text = detail.TextContent;
                            if (text.Contains("Живлення") && text.Contains("Вт"))
                            {
                                var parts = text.Split(' ');
                                tdp = parts.Last().Replace("Вт", "").Trim();
                            }
                        }

                        if (string.IsNullOrEmpty(tdp)) continue;

                        gpus.Add(new Gpu
                        {
                            Name = name,
                            Link = link,
                            Tdp = tdp
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"No data about GPU: {e.Message}, returning current result");
                        goto EndScraping; // Exit loops and proceed to file writing
                    }
                }

                var nextButton = document.QuerySelector("a.ib.pager-next");
                pageLink = nextButton != null ? "https://ek.ua" + nextButton.GetAttribute("href") : null;
                if (pageLink == null) Console.WriteLine("End of catalog reached.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Scraping interrupted: {e.Message}");
        }
        finally
        {
            await browser.CloseAsync();
        }

        EndScraping:; // Label for goto

        using (var writer = new StreamWriter("data/gpus.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(gpus);
        }
        
        Console.WriteLine($"Successfully saved {gpus.Count} GPUs to 'data/gpus.csv'.");
    }
}
