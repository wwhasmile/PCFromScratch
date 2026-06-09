using System.Globalization;
using System.Text.RegularExpressions;

using AngleSharp;
using AngleSharp.Dom;
using CsvHelper;
using Microsoft.Playwright;
using PCFromScratch.DBModels;
using PCFromScratch.Common;

namespace PCFromScratch.Scrapers;

//For now install Playwright browsers by writing in code Microsoft.Playwright.Program.Main(new[] { "install" }); during first launch, then I'm planning to add browsers in release app
public class CpuScraper
{
    public static async Task<List<Cpu>> GetCpus()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });
        var page = await browser.NewPageAsync();

        var cpus = new List<Cpu>();

        var pageLink = "https://ek.ua/ua/list/186/";
        
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
                        (string socket, string ram) = GetModelDetails(detailsDiv);

                        var confList = modelInfo.QuerySelector("div.m-c-f1-pl--button");
                        
                        var image = card.QuerySelector("img").GetAttribute("src");

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
                                
                                string submodelName = item.TextContent.Replace("\n", " ").Trim();
                                submodelName = Regex.Replace(submodelName, @"\s+", " ");
                                var tdp = await GetSubmodelDetails(cardLocator);
                                var packing = submodelName.EndsWith("OEM") ? "oem" : "box";
                                await CreateAndAddCpu(cpus, uSpan + ' ' + submodelName, packing, socket, tdp, ram, image, cardLocator);
                            }
                        }
                        else
                        {
                            var nameTag = modelInfo.QuerySelector("span.u")?.TextContent + " " + modelInfo.QuerySelector("span.list-conf-name.ib.nobr")?.TextContent;
                            var packing = nameTag.EndsWith("OEM") ? "oem" : "box";
                            var cardLocator = page.Locator("table.model-short-block").Nth(i);
                            var tdp = await GetSubmodelDetails(cardLocator);
                            await CreateAndAddCpu(cpus, uSpan, packing, socket, tdp, ram, image, cardLocator);
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

        return cpus;
    }

    private static (string, string) GetModelDetails(IElement? detailsDiv)
    {
        string socket = "", ram = "";
        if (detailsDiv == null) return (socket, ram);

        foreach (var detail in detailsDiv.ChildNodes)
        {
            var text = detail.TextContent;
            if (text.Contains("Socket"))
                socket = detail.ChildNodes[1].TextContent;
            else if (text.Contains("ОЗП"))
                ram = detail.ChildNodes[1].TextContent;
        }
        return (socket, ram);
    }
    
    private static async Task<int> GetSubmodelDetails(ILocator detailsDiv)
    {
        var details = (await detailsDiv.InnerTextAsync()).Split('\n');
        foreach (var detail in details)
        {
            if (detail.Contains("TDP"))
            {
                int.TryParse(Regex.Replace(detail, "[^0-9]", ""), out var tdp);
                return tdp;
            }
        }
        return 0;
    }

    private static async Task CreateAndAddCpu(List<Cpu> list, string model, string? packing, string socket, int tdp, string ram, string? image, ILocator card)
    {
        var priceInfo = card.Locator("td.model-hot-prices-td");
        var (minPr, maxPr, offers) = await BaseScraper.GetPriceInfoAsync(priceInfo);

        var link = "https://ek.ua" + await card.Locator("div.model-short-links a").Filter(new() { HasText = "Ціни" }).First.GetAttributeAsync("link");

        var ramSplitted = ram.Split(", ");
        string ramGen = "";
        int ramFreq = 0;

        if (ramSplitted.Length > 1 || !ram.StartsWith("макс"))
        {
            var ramInfo = ramSplitted.Last().Replace("МГц", "").Trim().Split(" ");
            ramGen = ramInfo[0];
            int.TryParse(ramInfo[1], out ramFreq);
        }
        
        list.Add(new Cpu
        {
            Id = Guid.NewGuid(),
            Name = model,
            Link = link,
            Socket = socket,
            Tdp = tdp,
            RamGen = ramGen,
            RamFrequency = ramFreq,
            Packing = packing?.ToLower() == "box" ? CpuPacking.Box : CpuPacking.OEM,
            ImageUrl = image,
            MaxPrice = maxPr,
            MinPrice = minPr,
            Offers = offers
        });
    }
}