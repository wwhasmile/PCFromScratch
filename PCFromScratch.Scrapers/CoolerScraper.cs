using System.Globalization;
using System.Text.RegularExpressions;

using AngleSharp;
using AngleSharp.Dom;
using CsvHelper;
using Microsoft.Playwright;
using PCFromScratch.DBModels;
using PCFromScratch.Common;

namespace PCFromScratch.Scrapers;

public class CoolerScraper
{
    public static async Task<List<Cooler>> GetCoolers()
    {
        Microsoft.Playwright.Program.Main(new[] { "install" });
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            Timeout = 60000
        });
        var page = await browser.NewPageAsync();

        var coolers = new List<Cooler>();

        var pageLink = "https://ek.ua/ua/ek-list.php?presets_=7154%2C35285%2C35286&katalog_=303&pf_=1&sc_id_=980&order_=pop&save_podbor_=1";
        
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
                        (int tdp, var intelSockets, var amdSockets, int height, CoolerType type) = GetModelDetails(detailsDiv);
                        if (tdp == 0) continue;
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
                                await CreateAndAddCooler(coolers, tdp, intelSockets, amdSockets, height, type, cardLocator);
                            }
                        }
                        else await CreateAndAddCooler(coolers, tdp, intelSockets, amdSockets, height, type, page.Locator("table.model-short-block").Nth(i));
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

        return coolers;
    }

    private static (int, List<string>, List<string>, int, CoolerType) GetModelDetails(IElement? detailsDiv)
    {
        int tdp = 0, height = 0;
        var intelSockets = new List<string>();
        var amdSockets = new List<string>();
        CoolerType type = CoolerType.ActiveCooler;

        if (detailsDiv == null) return (tdp, intelSockets, amdSockets, height, type);

        foreach (var detail in detailsDiv.ChildNodes)
        {
            var text = detail.TextContent;
            if (text.Contains("Тип"))
                type = detail.ChildNodes[1].TextContent.Contains("водяне") ? CoolerType.WaterCooler : CoolerType.ActiveCooler;
            else if (text.Contains("TDP"))
                int.TryParse(detail.ChildNodes[1].TextContent.Replace("Вт", "").Trim(), out tdp);
            else if (text.Contains("Socket Intel"))
            {
                var socketsIntel = Regex.Split(detail.ChildNodes[1].TextContent.Trim(), @"/|,").Select(s => s.Trim()); 
                intelSockets.AddRange(socketsIntel);
            }
            else if (text.Contains("Socket AMD"))
            {
                var socketsAmd = Regex.Split(detail.ChildNodes[1].TextContent.Trim(), @"/|,").Select(s => s.Trim());
                amdSockets.AddRange(socketsAmd);
            }
            else if (text.Contains("Висота"))
                int.TryParse(detail.ChildNodes[1].TextContent.Replace("мм", "").Trim(), out height);
        }
        return (tdp, intelSockets, amdSockets, height, type);
    }

    private static async Task<(int, int, int, int)> GetSubmodelDetails(ILocator detailsDiv)
    {
        int fanCount = 1, radius = 0, thickness = 0, speed = 0;
        var details = (await detailsDiv.InnerTextAsync()).Split('\n');
        foreach (var textContent in details)
        {
            var text = Regex.Replace(textContent, "\\s", " ");
            if (text.Contains("Вентилятор"))
            {
                var countStr = Regex.Match(text, @"\d+ шт").Value;
                if (!string.IsNullOrEmpty(countStr)) int.TryParse(countStr.Replace("шт", "").Trim(), out fanCount);
                var radiusStr = Regex.Match(text, @", \d+ мм").Value;
                if (!string.IsNullOrEmpty(radiusStr)) int.TryParse(radiusStr.Replace(", ", "").Replace("мм", "").Trim(), out radius);
                var thicknessStr = Regex.Match(text, @"товщина \d+ мм").Value;
                if (!string.IsNullOrEmpty(thicknessStr)) int.TryParse(thicknessStr.Replace("товщина", "").Replace("мм", "").Trim(), out thickness);
                var speedStr = Regex.Match(text, @"\d+ об/хв").Value;
                if (!string.IsNullOrEmpty(speedStr)) int.TryParse(speedStr.Replace("об/хв", "").Trim(), out speed);
                return (fanCount, radius, thickness, speed);
            }
        }
        return (fanCount, radius, thickness, speed);
    }
    
    private static async Task CreateAndAddCooler(List<Cooler> list, int tdp, List<string> intelSockets, List<string> amdSockets, int height, CoolerType type, ILocator card)
    {   
        var name = (await card.Locator("td.model-short-info span.u").InnerTextAsync()).Trim();
        var imageUrl = await card.Locator("img").First.GetAttributeAsync("src");
        
        var priceInfo = card.Locator("td.model-hot-prices-td");
        var (minPr, maxPr, offers) = await BaseScraper.GetPriceInfoAsync(priceInfo);
        
        var (fanCount, radius, thickness, speed) = await GetSubmodelDetails(card.Locator("div.m-s-f2"));
        var link = "https://ek.ua" + await card.Locator("div.model-short-links a").
            Filter(new() { HasText = "Ціни" }).First.GetAttributeAsync("link");
        
        list.Add(new Cooler
        {
            Id = Guid.NewGuid(),
            Name = name,
            Link = link,
            Tdp = tdp,
            IntelSockets = intelSockets,
            AmdSockets = amdSockets,
            FanCount = fanCount,
            Radius = radius,
            Thickness = thickness,
            Speed = speed,
            Height = height,
            Type = type,
            ImageUrl = imageUrl,
            MaxPrice = maxPr,
            MinPrice = minPr,
            Offers = offers
        });
    }
}