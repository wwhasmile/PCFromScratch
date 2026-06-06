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
                        var card = cards[i];
                        var modelInfo = card.QuerySelector("td.model-short-info");
                        if (modelInfo == null) continue;

                        var detailsDiv = modelInfo.QuerySelector("div.m-s-f2");
                        string format = GetModelDetails(detailsDiv);

                        var confList = modelInfo.QuerySelector("div.m-c-f1-pl--button");

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
                                if (confItems.Count > 4) await Task.Delay(1000);
                                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                                await CreateAndAddHdd(hdds, format, cardLocator);
                            }
                        }
                        else
                        {
                            await CreateAndAddHdd(hdds, format, page.Locator("table.model-short-block").Nth(i));
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

    private static async Task<int> GetCapacity(ILocator detailsDiv)
    {
        var details = (await detailsDiv.InnerTextAsync()).Split('\n');
        foreach (var detail in details)
        {
            var text = Regex.Replace(detail, "\\s", " ");
            if (text.Contains("Ємність"))
            {
                var capacityStr = detail.Trim();
                int capacity = int.Parse(Regex.Replace(capacityStr, "[^0-9]", ""));
                if (capacityStr.Contains("TB"))
                {
                    return capacity * 1024;
                }
                return capacity;
            }
        }
        return 0;
    }

private static async Task CreateAndAddHdd(List<InternalDrive> list, string format, ILocator card)
    {
        var name = (await card.Locator("td.model-short-info span.u").InnerTextAsync()).Trim();
        var priceInfo = card.Locator("td.model-hot-prices-td");
        var (minPr, maxPr, offers) = await BaseScraper.GetPriceInfoAsync(priceInfo);
        var image = await card.Locator("img").First.GetAttributeAsync("src");
        int capacity = await GetCapacity(card.Locator("div.m-s-f2"));
        var link = "https://ek.ua" + await card.Locator("div.model-short-links a").Filter(new() { HasText = "Ціни" }).First.GetAttributeAsync("link");

        list.Add(new InternalDrive
        {
            Id = Guid.NewGuid(),
            Name = name,
            Link = link,
            Capacity = capacity,
            Type = "HDD",
            Format = format,
            Port = "SATA",
            ImageUrl = image,
            MaxPrice = maxPr,
            MinPrice = minPr,
            Offers = offers
        });
    }
}