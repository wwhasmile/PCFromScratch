using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using CsvHelper;
using Microsoft.Playwright;
using PCFromScratch.DBModels;

namespace PCFromScratch.Scrapers;

public class RamScraper
{
    public static async Task<List<Ram>> GetRams()
    {
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
                
                var content = await page.ContentAsync();
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(content));

                var cards = document.QuerySelectorAll("table.model-short-block");

                for (int i = 0; i < cards.Length; i++)
                {
                    try
                    {
                        var card = cards[i];
                        var modelInfo = card.QuerySelector("td.model-short-info");
                        if (modelInfo == null) continue;
                        
                        var uSpan = modelInfo.QuerySelector("span.u")?.TextContent.Trim();
                        if (string.IsNullOrEmpty(uSpan)) continue;
                        
                        var detailsDiv = modelInfo.QuerySelector("div.m-s-f2");
                        (string capacityStr, string voltageStr) = GetModelDetails(detailsDiv);

                        var confList = modelInfo.QuerySelector("div.m-c-f1-pl--button");
                        
                        if (confList != null)
                        {
                            var confItems = confList.QuerySelectorAll("span.ib").ToList();
                            
                            for (var j = 0; j < confItems.Count; j++)
                            {
                                var item = confItems[j];
                                if (item.ClassList.Contains("out-of-stock")) continue;

                                string submodelName = item.TextContent.Replace("\n", " ").Trim();
                                submodelName = System.Text.RegularExpressions.Regex.Replace(submodelName, @"\s+", " ");

                                var cardLocator = page.Locator("table.model-short-block").Nth(i);
                                var submodelLocator = cardLocator.Locator("div.m-c-f1-pl--button span.ib").Nth(j);
                                await submodelLocator.ClickAsync();
                                if (confItems.Count > 4) await Task.Delay(1000);
                                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                                await CreateAndAddRam(rams, uSpan, submodelName, capacityStr, voltageStr, cardLocator);
                            }
                        }
                        else await CreateAndAddRam(rams, uSpan, null, capacityStr, voltageStr, page.Locator("table.model-short-block").Nth(i));
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

        return rams;
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

    private static async Task<(string, string)> GetSubmodelDetails(ILocator detailsDiv)
    {
        string ramGen = "", ramFreq = "";
        foreach (var detail in (await detailsDiv.InnerTextAsync()).Split('\n'))
        {
            if (detail.Contains("Швидкість"))
            {
                var genFreqStr = detail.Split(':')[1].Trim();
                var dashSplit = genFreqStr.Split('-');
                if (dashSplit.Length >= 2)
                {
                    ramGen = dashSplit[0].Trim();
                    ramFreq = dashSplit[1].Trim();
                }
            }
        }
        return (ramGen, ramFreq);
    }

    private static async Task CreateAndAddRam(List<Ram> list, string model, string? submodel, string capacityStr,
        string voltageStr, ILocator card)
    {
        var priceInfo = card.Locator("td.model-hot-prices-td");
        var (minPr, maxPr, offers)= await BaseScraper.GetPriceInfoAsync(priceInfo);
        (string ramGen, string ramFreq) = await GetSubmodelDetails(card.Locator("div.m-s-f2"));
        var image = await card.Locator("img").First.GetAttributeAsync("src");
        var link = "https://ek.ua" + await card.Locator("div.model-short-links a").
            Filter(new() { HasText = "Ціни" }).First.GetAttributeAsync("link");
                                
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
            ImageUrl = image,
            Offers = offers,
            MinPrice = minPr,
            MaxPrice = maxPr,
            Sticks = sticks,
            Voltage = voltage
        });
    }
}