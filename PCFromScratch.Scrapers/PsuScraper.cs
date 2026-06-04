using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using CsvHelper;
using Microsoft.Playwright;
using PCFromScratch.DBModels;
using PCFromScratch.Common;

namespace PCFromScratch.Scrapers;

public class PsuScraper
{
    private static readonly string FilePath = "data/psus.csv";

    public static async Task GetPsus()
    {
        BaseScraper.CreatePath(FilePath);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });
        var page = await browser.NewPageAsync();

        var psus = new List<Psu>();

        var pageLink = "https://ek.ua/ua/list/351/";
        
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

                        var imageTask = page.GetByAltText($"Блок живлення {uSpan}").ScreenshotAsync();
                        
                        var detailsDiv = modelInfo.QuerySelector("div.m-s-f2");
                        (int power, PsuFormFactor formFactor, PsuModularity modularity) = GetModelDetails(detailsDiv);

                        var confList = modelInfo.QuerySelector("div.m-c-f1-pl--button");
                        
                        var image = await imageTask;

                        if (confList != null)
                        {
                            var confItems = confList.QuerySelectorAll("span.ib").ToList();
                            
                            for (var j = 0; j < confItems.Count; j++)
                            {
                                var item = confItems[j];
                                if (item.ClassList.Contains("out-of-stock")) continue;
                                
                                if (!item.ClassList.Contains("current"))
                                {
                                    var cardLocator = page.Locator("table.model-short-block").Nth(i);
                                    var submodelLocator = cardLocator.Locator("div.m-c-f1-pl--button span.ib").Nth(j);
                                    await submodelLocator.ClickAsync();
                                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                                }

                                string powerConnector = GetPowerConnectors(card.QuerySelector("div.m-s-f2"));
                                CreateAndAddPsu(psus, uSpan, power, formFactor, modularity, powerConnector, image, card);
                            }
                        }
                        else
                        {
                            string powerConnector = GetPowerConnectors(card.QuerySelector("div.m-s-f2"));
                            CreateAndAddPsu(psus, uSpan, power, formFactor, modularity, powerConnector, image, card);
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

    private static (int, PsuFormFactor, PsuModularity) GetModelDetails(IElement? detailsDiv)
    {
        int power = 0;
        PsuFormFactor formFactor = PsuFormFactor.ATX;
        PsuModularity modularity = PsuModularity.NotModular;

        if (detailsDiv == null) return (power, formFactor, modularity);

        foreach (var detail in detailsDiv.ChildNodes)
        {
            var text = detail.TextContent;
            if (text.Contains("Потужність"))
                int.TryParse(detail.ChildNodes[1].TextContent.Replace("Вт", "").Trim(), out power);
            else if (text.Contains("Форм-фактор"))
            {
                var textContent = detail.ChildNodes[1].TextContent;
                if (textContent.Contains("SFX"))
                    formFactor = PsuFormFactor.SFX;
                if (textContent.Contains("модульний"))
                {
                    modularity = PsuModularity.Modular;
                }
                else if (textContent.Contains("напівмодульний"))
                {
                    modularity = PsuModularity.SemiModular;
                }
            }
        }
        return (power, formFactor, modularity);
    }

    private static PsuLevel GetPsuLevel(IElement card)
    {
        var badgesDiv = card.QuerySelector("div.m-s-f1");
        if (badgesDiv != null)
        {
            var badge = badgesDiv.ChildNodes.FirstOrDefault(n => n.TextContent.Contains("80+"));
            if (badge != null)
            {
                var levelString = badge.TextContent;
                return levelString switch
                {
                    "80+ Bronze" => PsuLevel.Bronze,
                    "80+ Silver" => PsuLevel.Silver,
                    "80+ Gold" => PsuLevel.Gold,
                    "80+ Platinum" => PsuLevel.Platinum,
                    "80+ Titanium" => PsuLevel.Titanium,
                    "80+" => PsuLevel.Standard,
                    _ => PsuLevel.None
                };
            }
        }
        return PsuLevel.None;
    }

    private static string GetPowerConnectors(IElement? detailsDiv)
    {
        if (detailsDiv == null) return string.Empty;
        foreach (var detail in detailsDiv.ChildNodes)
        {
            var text = detail.TextContent;
            if (text.Contains("Живлення"))
            {
                if (detail.ChildNodes.Length > 1)
                {
                    return detail.ChildNodes[1].TextContent.Trim();
                }
            }
        }
        return string.Empty;
    }
    
    private static void CreateAndAddPsu(List<Psu> list, string model, int power, PsuFormFactor formFactor, PsuModularity modularity, string powerConnector, byte[] image, IElement card)
    {
        var priceInfo = card.QuerySelector("td.model-hot-prices-td");
        var (priceRange, offers) = BaseScraper.GetPriceInfo(priceInfo);

        var link = "https://ek.ua" + card.QuerySelector("div.model-short-links").QuerySelectorAll("a")
            .Where(n => n.TextContent.Contains("Ціни")).FirstOrDefault().GetAttribute("link");
        
        var level = GetPsuLevel(card);

        list.Add(new Psu
        {
            Id = Guid.NewGuid(),
            Name = model,
            Link = link,
            Power = power,
            Level = level,
            FormFactor = formFactor,
            Modularity = modularity,
            PowerConnector = powerConnector,
            Image = image,
            PriceRange = priceRange,
            Offers = offers
        });
    }
}