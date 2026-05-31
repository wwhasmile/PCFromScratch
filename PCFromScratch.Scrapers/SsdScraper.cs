using System.Globalization;
using AngleSharp;
using CsvHelper;
using Microsoft.Playwright;

using PCFromScratch.Scrapers.CSVModels;

namespace PCFromScratch.Scrapers;

public class SsdScraper
{
    private static readonly string FilePath = "data/ssd.csv";

    public static async Task GetSsds()
    {
        if (!File.Exists(FilePath))
        {
            string? directoryName = Path.GetDirectoryName(FilePath);
            if (directoryName is not null && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            File.Create(FilePath).Close();
        }

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });
        var page = await browser.NewPageAsync();

        var ssds = new List<Ssd>();
        var random = new Random();
        
        var pageLink = "https://ek.ua/ua/list/61/pr-3680/";
        
        try
        {
            while (pageLink != null)
            {
                Console.WriteLine($"Retrieving page: {pageLink}");
                await page.GotoAsync(pageLink);

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
                        var modelName = card.QuerySelector("span.u")?.TextContent.Trim();
                        if (string.IsNullOrEmpty(modelName)) continue;

                        var mainLink = "https://ek.ua" + card.QuerySelector("a.model-short-title.no-u")?.GetAttribute("href");
                        
                        var detailsDiv = card.QuerySelector("div.m-s-f2");
                        string format = "";

                        // Extract main details, specifically looking for Format/Form-factor which is shared across submodels
                        if (detailsDiv != null)
                        {
                            var details = detailsDiv.ChildNodes;
                            foreach (var detail in details)
                            {
                                var text = detail.TextContent;
                                if (text.Contains("Форм-фактор") || text.Contains("Формат"))
                                {
                                     if (detail.ChildNodes.Length > 1) format = detail.ChildNodes[1].TextContent.Replace("\"", "").Trim();
                                }
                            }
                        }

                        var confList = card.QuerySelector("div.m-c-f1-pl--button");
                        
                        if (confList != null)
                        {
                            var confItems = confList.QuerySelectorAll("span.ib");
                            
                            foreach (var item in confItems)
                            {
                                if (item.ClassList.Contains("out-of-stock")) continue;

                                string submodelName = item.TextContent.Replace("\n", " ").Trim();
                                submodelName = System.Text.RegularExpressions.Regex.Replace(submodelName, @"\s+", " ");
                                
                                string capacity = "";
                                
                                // Parse capacity from submodel name. It usually starts with it (e.g., "1 TB", "500 GB", "2 TB M.2")
                                if (submodelName.Contains("TB") || submodelName.Contains("GB"))
                                {
                                     var parts = submodelName.Split(new[] { "TB", "GB" }, StringSplitOptions.None);
                                     if (parts.Length > 0)
                                     {
                                         string unit = submodelName.Contains("TB") ? "TB" : "GB";
                                         capacity = parts[0].Trim() + " " + unit;
                                     }
                                }
                                
                                string currentLink;
                                if (item.ClassList.Contains("current"))
                                {
                                    currentLink = mainLink;
                                }
                                else
                                {
                                    var aTag = item.QuerySelector("a");
                                    currentLink = aTag != null ? "https://ek.ua" + aTag.GetAttribute("href") : mainLink;
                                }

                                ssds.Add(new Ssd
                                {
                                    Model = modelName,
                                    Submodel = submodelName,
                                    Link = currentLink,
                                    Capacity = capacity,
                                    Format = format
                                });
                            }
                        }
                        else
                        {
                           // Fallback if no submodels exist
                           string fallbackCapacity = "";
                           if (detailsDiv != null)
                           {
                               var details = detailsDiv.ChildNodes;
                               foreach (var detail in details)
                               {
                                   if (detail.TextContent.Contains("Ємність") || detail.TextContent.Contains("Об'єм"))
                                   {
                                        if (detail.ChildNodes.Length > 1) fallbackCapacity = detail.ChildNodes[1].TextContent.Trim();
                                   }
                               }
                           }

                           ssds.Add(new Ssd
                           {
                               Model = modelName,
                               Link = mainLink,
                               Capacity = fallbackCapacity,
                               Format = format
                           });
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Skipping a malformed card. Error: {e.Message}");
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

        using (var writer = new StreamWriter(FilePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(ssds);
        }
        
        Console.WriteLine($"Successfully saved {ssds.Count} SSDs to '{FilePath}'.");
    }
}