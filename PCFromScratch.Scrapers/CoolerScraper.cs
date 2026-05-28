using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using CsvHelper;
using Microsoft.Playwright;

using PCFromScratch.Scrapers.CSVModels;

namespace PCFromScratch.Scrapers;

public class CoolerScraper
{
    public static async Task GetCoolers()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false // Set to false to see the browser UI
        });
        var page = await browser.NewPageAsync();

        var coolers = new List<Cooler>();
        var random = new Random();

        var pageLink = "https://ek.ua/ua/ek-list.php?presets_=7154%2C35285%2C35286&katalog_=303&pf_=1&sc_id_=980&order_=pop&save_podbor_=1";
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
                        var nameTag = card.QuerySelector("span.u")?.TextContent;
                        if (string.IsNullOrEmpty(nameTag)) continue;

                        var link = "https://ek.ua" + card.QuerySelector("a.model-short-title.no-u")?.GetAttribute("href");

                        var detailsDiv = card.QuerySelector("div.m-s-f2");
                        if (detailsDiv == null) continue;

                        var details = detailsDiv.ChildNodes;
                        string? tdp = null;
                        string? socketIntel = null;
                        string? socketAmd = null;
                        string? fan = null;
                        string? type = null;
                        string? height = null;

                        foreach (var detail in details)
                        {
                            var text = detail.TextContent;

                            if (text.Contains("TDP"))
                            {
                                if (detail.ChildNodes.Length > 1) tdp = detail.ChildNodes[1].TextContent.Replace("Вт", "").Trim();
                            }
                            else if (text.Contains("Socket Intel"))
                            {
                                if (detail.ChildNodes.Length > 1) socketIntel = detail.ChildNodes[1].TextContent.Trim();
                            }
                            else if (text.Contains("Socket AMD"))
                            {
                                if (detail.ChildNodes.Length > 1) socketAmd = detail.ChildNodes[1].TextContent.Trim();
                            }
                            else if (text.Contains("Вентилятор"))
                            {
                                if (detail.ChildNodes.Length > 1) fan = detail.ChildNodes[1].TextContent.Trim();
                            }
                            else if (text.Contains("Тип"))
                            {
                                if (detail.ChildNodes.Length > 1) 
                                {
                                    type = detail.ChildNodes[1].TextContent.Replace(", для процесора", "").Trim();
                                }
                            }
                            else if (text.Contains("Висота"))
                            {
                                if (detail.ChildNodes.Length > 1) height = detail.ChildNodes[1].TextContent.Replace("мм", "").Trim();
                            }
                        }

                        // We only add it if we found a TDP, as it's crucial for matching with CPUs.
                        if (string.IsNullOrEmpty(tdp)) continue;

                        coolers.Add(new Cooler
                        {
                            Name = nameTag,
                            Link = link,
                            Tdp = tdp,
                            SocketIntel = socketIntel,
                            SocketAmd = socketAmd,
                            Fan = fan,
                            Type = type,
                            Height = height
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Skipping a malformed card. Error: {e.Message}");
                        continue;
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

        // Ensure data directory exists
        var dataDir = "data";
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }

        using (var writer = new StreamWriter("data/coolers.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(coolers);
        }
        
        Console.WriteLine($"Successfully saved {coolers.Count} coolers to 'data/coolers.csv'.");
    }
}
