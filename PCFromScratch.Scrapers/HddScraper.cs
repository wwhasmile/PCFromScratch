using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using CsvHelper;
using Microsoft.Playwright;
using PCFromScratch.DBModels;

namespace PCFromScratch.Scrapers;

public class HddScraper
{
    private static readonly string FilePath = "data/hdds.csv";

    public static async Task GetHdds()
    {
        BaseScraper.CreatePath(FilePath);

        using var playwright = await Playwright.CreateAsync();
        await using var browser =
            await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var page = await browser.NewPageAsync();

        var hdds = new List<InternalDrive>();

        var pageLink =
            "https://ek.ua/ua/ek-list.php?presets_=3573%2C3590&katalog_=190&pf_=1&sc_id_=980&order_=pop&save_podbor_=1";

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
                        var modelInfo = cards[i].QuerySelector("td.model-short-info");
                        if (modelInfo == null) continue;

                        var uSpan = modelInfo.QuerySelector("span.u")?.TextContent.Trim();
                        if (string.IsNullOrEmpty(uSpan)) continue;

                        var imageTask = page.GetByAltText(uSpan).ScreenshotAsync();

                        var detailsDiv = modelInfo.QuerySelector("div.m-s-f2");
                        string format = GetModelDetails(detailsDiv);

                        var confList = modelInfo.QuerySelector("div.m-c-f1-pl--button");

                        var image = await imageTask;

                        if (confList != null)
                        {
                            var confItems = confList.QuerySelectorAll("span.ib").ToList();

                            for (var j = 0; j < confItems.Count; j++)
                            {
                                var item = confItems[j];
                                if (item.ClassList.Contains("out-of-stock")) continue;
                                
                                var cardLocator = page.Locator("table.model-short-block").Nth(i);
                                var submodelLocator = cardLocator.Locator("div.m-c-f1-pl--button span.ib").Nth(j);
                                await submodelLocator.ClickAsync();
                                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                                int capacity = GetCapacity(cards[i].QuerySelector("div.m-s-f2"));
                                CreateAndAddHdd(hdds, uSpan, capacity, format, image, cards[i]);
                            }
                        }
                        else
                        {
                            int capacity = GetCapacity(cards[i].QuerySelector("div.m-s-f2"));
                            CreateAndAddHdd(hdds, uSpan, capacity, format, image, cards[i]);
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
            csv.WriteRecords(hdds);
        }

        Console.WriteLine($"Successfully saved {hdds.Count} HDDs to '{FilePath}'.");
    }

    private static string GetModelDetails(IElement? detailsDiv)
    {
        if (detailsDiv == null) return string.Empty;

        foreach (var detail in detailsDiv.ChildNodes)
        {
            var text = detail.TextContent;
            if (text.Contains("Формат"))
            {
                var format = detail.ChildNodes[1].TextContent.Replace("\"", "").Trim();
                return format;
            }
        }

        return string.Empty;
    }

    private static int GetCapacity(IElement? detailsDiv)
    {
        if (detailsDiv == null) return 0;
        foreach (var detail in detailsDiv.ChildNodes)
        {
            var text = detail.TextContent;
            if (text.Contains("Ємність"))
            {
                if (detail.ChildNodes.Length > 1)
                {
                    var capacityStr = detail.ChildNodes[1].TextContent.Trim();
                    int capacity = int.Parse(Regex.Replace(capacityStr, "[^0-9]", ""));
                    if (capacityStr.Contains("TB"))
                    {
                        return capacity * 1024;
                    }
                    return capacity;
                }
            }
        }
        return 0;
    }

private static void CreateAndAddHdd(List<InternalDrive> list, string model, int capacity, string format, byte[] image, IElement card)
    {
        var priceInfo = card.QuerySelector("td.model-hot-prices-td");
        var (priceRange, offers) = BaseScraper.GetPriceInfo(priceInfo);

        var link = "https://ek.ua" + card.QuerySelector("div.model-short-links").QuerySelectorAll("a")
            .Where(n => n.TextContent.Contains("Ціни")).FirstOrDefault().GetAttribute("link");

        list.Add(new InternalDrive
        {
            Id = Guid.NewGuid(),
            Name = model,
            Link = link,
            Capacity = capacity,
            Type = "HDD",
            Format = format,
            Port = "SATA",
            Image = image,
            PriceRange = priceRange,
            Offers = offers
        });
    }
}