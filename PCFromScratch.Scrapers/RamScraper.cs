using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using CsvHelper;
using Microsoft.Playwright;
using PCFromScratch.DBModels;

namespace PCFromScratch.Scrapers;

public class RamScraper
{
    private static readonly string FilePath = "data/ram.csv";

    public static async Task GetRams()
    {
        BaseScraper.CreatePath(FilePath);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });
        var page = await browser.NewPageAsync();

        var rams = new List<Ram>();

        var pageLink = "https://ek.ua/ua/ek-list.php?katalog_=188&presets_=4480";
        
        try
        {
            while (pageLink != null)
            {
                Console.WriteLine($"Retrieving page: {pageLink}");
                await page.GotoAsync(pageLink);

                await page.EvaluateAsync(@"
                    let cards = document.querySelectorAll('table.model-short-block');
                    for (let i = 0; i < cards.length; i++) {
                        cards[i].id = 'card-for-scraping-' + i;
                    }
                ");

                var content = await page.ContentAsync();
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(content));

                var cards = document.QuerySelectorAll("table.model-short-block");

                foreach (var card in cards)
                {
                    try
                    {
                        var cardId = card.Id;
                        var cardLocator = page.Locator($"#{cardId}");

                        var modelInfo = card.QuerySelector("td.model-short-info");
                        if (modelInfo == null) continue;
                        
                        var uSpan = modelInfo.QuerySelector("span.u")?.TextContent.Trim();
                        if (string.IsNullOrEmpty(uSpan)) continue;

                        var imageTask = page.GetByAltText($"Оперативна пам'ять {uSpan}").ScreenshotAsync();
                        
                        var detailsDiv = modelInfo.QuerySelector("div.m-s-f2");
                        (string capacityStr, string voltageStr) = GetModelDetails(detailsDiv);

                        var confList = modelInfo.QuerySelector("div.m-c-f1-pl--button");

                        var image = await imageTask;
                        
                        if (confList != null)
                        {
                            var confItems = confList.QuerySelectorAll("span.ib").ToList();
                            
                            for (var i = 0; i < confItems.Count; i++)
                            {
                                var item = confItems[i];
                                if (item.ClassList.Contains("out-of-stock")) continue;

                                string submodelName = item.TextContent.Replace("\n", " ").Trim();
                                submodelName = System.Text.RegularExpressions.Regex.Replace(submodelName, @"\s+", " ");

                                if (!item.ClassList.Contains("current"))
                                {
                                    var submodelLocator = cardLocator.Locator("div.m-c-f1-pl--button span.ib").Nth(i);
                                    await submodelLocator.ClickAsync();
                                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                                }
                                CreateAndAddRam(rams, uSpan, submodelName, capacityStr, voltageStr, image, card);
                            }
                        }
                        else CreateAndAddRam(rams, uSpan, null, capacityStr, voltageStr, image, card);
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
            csv.WriteRecords(rams);
        }
        
        Console.WriteLine($"Successfully saved {rams.Count} RAM modules to '{FilePath}'.");
    }

    private static (string, string) GetModelDetails(IElement? detailsDiv)
    {
        string capacityStr = "", voltageStr = "";
        if (detailsDiv == null) return (capacityStr, voltageStr);
        foreach (var detail in detailsDiv.ChildNodes)
        {
            var text = detail.TextContent;
            if (text.Contains("Об'єм"))
            {
                if (detail.ChildNodes.Length > 1) capacityStr = detail.ChildNodes[1].TextContent.Trim();
            }
            else if (text.Contains("Напруга"))
            {
                if (detail.ChildNodes.Length > 1) voltageStr = detail.ChildNodes[1].TextContent.Replace("В", "").Trim();
            }
        }
        return (capacityStr, voltageStr);
    }

    private static (string, string) GetSubmodelDetails(IElement? detailsDiv)
    {
        string ramGen = "", ramFreq = "";
        if (detailsDiv == null) return (ramGen, ramFreq);
        foreach (var detail in detailsDiv.ChildNodes)
        {
            var text = detail.TextContent;
            if (text.Contains("Швидкість"))
            {
                if (detail.ChildNodes.Length > 1)
                {
                    var genFreqStr = detail.ChildNodes[1].TextContent.Trim();
                    var dashSplit = genFreqStr.Split('-');
                    if (dashSplit.Length >= 2)
                    {
                        ramGen = dashSplit[0].Trim();
                        ramFreq = dashSplit[1].Trim();
                    }
                }
            }
        }
        return (ramGen, ramFreq);
    }

    private static void CreateAndAddRam(List<Ram> list, string model, string? submodel, string capacityStr,
        string voltageStr, byte[] image, IElement card)
    {
        var priceInfo = card.QuerySelector("td.model-hot-prices-td");
        var (priceRange, offers)= BaseScraper.GetPriceInfo(priceInfo);
        (string ramGen, string ramFreq) = GetSubmodelDetails(card.QuerySelector("div.m-s-f2"));

        var link = "https://ek.ua" + card.QuerySelector("div.model-short-links").QuerySelectorAll("a")
            .Where(n => n.TextContent.Contains("Ціни")).FirstOrDefault().GetAttribute("link");
                                
        var capacityParts = capacityStr.Split('х');
        int.TryParse(capacityParts.Length > 1 ? capacityParts[0].Trim() : "1", out var sticks);
        int.TryParse(System.Text.RegularExpressions.Regex.Match(capacityParts.Last(), @"\d+").Value, out var amount);
        float.TryParse(voltageStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var voltage);
        int.TryParse(ramFreq, out var frequency);
        
        list.Add(new Ram
        {
            Id = Guid.NewGuid(),
            Model = model,
            Submodel = submodel,
            Link = link,
            Amount = amount,
            Frequency = frequency,
            Generation = ramGen,
            Image = image,
            Offers = offers,
            PriceRange = priceRange,
            Sticks = sticks,
            Voltage = voltage
        });
    }
}