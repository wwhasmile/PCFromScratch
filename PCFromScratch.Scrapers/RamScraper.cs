using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using CsvHelper;
using Microsoft.Playwright;

using PCFromScratch.Scrapers.CSVModels;

namespace PCFromScratch.Scrapers;

public class RamScraper
{
    private static readonly string FilePath = "data/ram.csv";

    public static async Task GetRams()
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

        var rams = new List<Ram>();
        var random = new Random();

        var pageLink = "https://ek.ua/ua/ek-list.php?katalog_=188&presets_=4480";
        
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
                        var uSpan = card.QuerySelector("span.u")?.TextContent.Trim();
                        if (string.IsNullOrEmpty(uSpan)) continue;

                        var mainLink = "https://ek.ua" + card.QuerySelector("a.model-short-title.no-u")?.GetAttribute("href");
                        
                        var detailsDiv = card.QuerySelector("div.m-s-f2");
                        string capacity = "", voltage = "";

                        if (detailsDiv != null)
                        {
                            var details = detailsDiv.ChildNodes;
                            foreach (var detail in details)
                            {
                                var text = detail.TextContent;
                                if (text.Contains("Об'єм"))
                                {
                                     if (detail.ChildNodes.Length > 1) capacity = detail.ChildNodes[1].TextContent.Trim();
                                }
                                else if (text.Contains("Напруга"))
                                {
                                     if (detail.ChildNodes.Length > 1) voltage = detail.ChildNodes[1].TextContent.Replace("В", "").Trim();
                                }
                            }
                        }

                        var confList = card.QuerySelector("div.m-c-f1-pl--button");
                        
                        if (confList != null)
                        {
                            var confItems = confList.QuerySelectorAll("span.ib");
                            
                            foreach (var item in confItems)
                            {
                                // Check for the 'out-of-stock' class
                                if (item.ClassList.Contains("out-of-stock"))
                                {
                                    continue; // Skip this submodel
                                }

                                string submodelName = item.TextContent.Replace("\n", " ").Trim();
                                submodelName = System.Text.RegularExpressions.Regex.Replace(submodelName, @"\s+", " ");
                                
                                string ramGen = "", ramFreq = "";
                                
                                var dashSplit = submodelName.Split('-');
                                if (dashSplit.Length >= 2)
                                {
                                    ramGen = dashSplit[0].Trim();
                                    ramFreq = dashSplit[1].Split(' ')[0].Trim(); 
                                }
                                
                                string link;
                                if (item.ClassList.Contains("current"))
                                {
                                    link = mainLink;
                                }
                                else
                                {
                                    var aTag = item.QuerySelector("a");
                                    link = aTag != null ? "https://ek.ua" + aTag.GetAttribute("href") : mainLink;
                                }

                                rams.Add(new Ram
                                {
                                    Model = uSpan,
                                    Submodel = submodelName,
                                    Link = link,
                                    Capacity = capacity,
                                    Voltage = voltage,
                                    Generation = ramGen,
                                    Frequency = ramFreq
                                });
                            }
                        }
                        else
                        {
                           rams.Add(new Ram
                           {
                               Model = uSpan,
                               Link = mainLink,
                               Capacity = capacity,
                               Voltage = voltage
                           });
                        }
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

        using (var writer = new StreamWriter(FilePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(rams);
        }
        
        Console.WriteLine($"Successfully saved {rams.Count} RAM modules to '{FilePath}'.");
    }
}
