using System.Globalization;
using AngleSharp;
using CsvHelper;
using Microsoft.Playwright;

using PCFromScratch.Scrapers.CSVModels;

namespace PCFromScratch.Scrapers;

public class PsuScraper
{
    private static readonly string FilePath = "data/psu.csv";

    public static async Task GetPsus()
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

        var psus = new List<Psu>();
        var random = new Random();

        var pageLink = "https://ek.ua/ua/list/351/";
        
        try
        {
            while (pageLink != null)
            {
                Console.WriteLine($"Retrieving page: {pageLink}");
                await page.GotoAsync(pageLink);

                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight / 2);");
                await Task.Delay(random.Next(1000, 2500));
                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight);");
                await Task.Delay(random.Next(3000, 6000));

                var content = await page.ContentAsync();
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(content));

                var cards = document.QuerySelectorAll("td.model-short-info");

                foreach (var card in cards)
                {
                    try
                    {
                        // We ONLY want the main name tag (span.u), ignoring the submodel info (span.list-conf-name)
                        var name = card.QuerySelector("span.u")?.TextContent.Trim();
                        if (string.IsNullOrEmpty(name)) continue;

                        var link = "https://ek.ua" + card.QuerySelector("a.model-short-title.no-u")?.GetAttribute("href");
                        
                        var detailsDiv = card.QuerySelector("div.m-s-f2");
                        string power = "", formFactor = "", connectors = "", powerConnectors = "", level80Plus = "";

                        // Extract 80+ Level
                        var badgesDiv = card.QuerySelector("div.m-s-f1");
                        if (badgesDiv != null)
                        {
                            var badges = badgesDiv.QuerySelectorAll("a");
                            foreach (var badge in badges)
                            {
                                var badgeText = badge.TextContent;
                                if (badgeText.Contains("80+"))
                                {
                                    level80Plus = badgeText.Trim();
                                    break;
                                }
                            }
                        }

                        if (detailsDiv != null)
                        {
                            var details = detailsDiv.ChildNodes;
                            foreach (var detail in details)
                            {
                                var text = detail.TextContent;
                                if (text.Contains("Потужність"))
                                {
                                     if (detail.ChildNodes.Length > 1) power = detail.ChildNodes[1].TextContent.Replace("Вт", "").Trim();
                                }
                                else if (text.Contains("Форм-фактор"))
                                {
                                     if (detail.ChildNodes.Length > 1) formFactor = detail.ChildNodes[1].TextContent.Trim();
                                }
                                else if (text.Contains("Живлення"))
                                {
                                     if (detail.ChildNodes.Length > 1) powerConnectors = detail.ChildNodes[1].TextContent.Trim();
                                }
                                else if (text.Contains("Конекторів"))
                                {
                                     if (detail.ChildNodes.Length > 1) connectors = detail.ChildNodes[1].TextContent.Trim();
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
                                
                                string currentPower = power;
                                string currentPowerConnectors = powerConnectors;
                                if (System.Text.RegularExpressions.Regex.IsMatch(submodelName, ".*80\\+ (Standard|Bronze|Silver|Gold|Platinum|Titanium)"))
                                {
                                    level80Plus = "80+" + submodelName.Split("80+")[1].Trim();
                                    submodelName = submodelName.Split("80+")[0];
                                }
                                else if (submodelName.Contains("без 80+"))
                                {
                                    submodelName = submodelName.Replace("без 80+", "").Trim();
                                }
                                else if (submodelName.Contains("80+"))
                                {
                                    level80Plus = "80+ Standard";
                                    submodelName = submodelName.Replace("80+", "").Trim();
                                }
                                if (submodelName.Contains("Вт"))
                                {
                                    var parts = submodelName.Split("Вт");
                                    currentPower = parts[0].Trim();
                                    
                                    // The rest of the string is the power connectors if available
                                    if (parts.Length > 1)
                                    {
                                        var subConnectors = parts[1].Trim();
                                        if (!string.IsNullOrEmpty(subConnectors))
                                        {
                                            currentPowerConnectors = subConnectors;
                                        }
                                    }
                                }
                                
                                string currentLink;
                                if (item.ClassList.Contains("current"))
                                {
                                    currentLink = link;
                                }
                                else
                                {
                                    var aTag = item.QuerySelector("a");
                                    currentLink = aTag != null ? "https://ek.ua" + aTag.GetAttribute("href") : link;
                                }

                                psus.Add(new Psu
                                {
                                    Name = name, // Main name only, e.g. "be quiet! Pure Power 13 M"
                                    Link = currentLink,
                                    Power = currentPower,
                                    FormFactor = formFactor,
                                    Connectors = connectors,
                                    PowerConnectors = currentPowerConnectors,
                                    Level80Plus = level80Plus
                                });
                            }
                        }
                        else
                        {
                           psus.Add(new Psu
                           {
                               Name = name,
                               Link = link,
                               Power = power,
                               FormFactor = formFactor,
                               Connectors = connectors,
                               PowerConnectors = powerConnectors,
                               Level80Plus = level80Plus
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
            csv.WriteRecords(psus);
        }
        
        Console.WriteLine($"Successfully saved {psus.Count} PSUs to '{FilePath}'.");
    }
}
