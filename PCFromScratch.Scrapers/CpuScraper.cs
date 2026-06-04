using System.Globalization;
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
    private static readonly string FilePath = "data/cpus.csv";
    public static async Task GetCpus()
    {
        BaseScraper.CreatePath(FilePath);

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
                        var modelInfo = card.QuerySelector("td.model-short-info");
                        if (modelInfo == null) continue;
                        
                        var uSpan = modelInfo.QuerySelector("span.u")?.TextContent.Trim();
                        if (string.IsNullOrEmpty(uSpan)) continue;

                        var imageTask = page.GetByAltText($"Процесор {uSpan}").ScreenshotAsync();
                        
                        var detailsDiv = modelInfo.QuerySelector("div.m-s-f2");
                        (string socket, int tdp, string ram) = GetModelDetails(detailsDiv);

                        var confList = modelInfo.QuerySelector("div.m-c-f1-pl--button");
                        
                        var image = await imageTask;

                        if (confList != null)
                        {
                            var confItems = confList.QuerySelectorAll("span.ib").ToList();
                            
                            for (var i = 0; i < confItems.Count; i++)
                            {
                                var item = confItems[i];
                                if (item.ClassList.Contains("out-of-stock")) continue;
                                
                                if (!item.ClassList.Contains("current"))
                                {
                                    var cardId = card.Id;
                                    var cardLocator = page.Locator($"#{cardId}");
                                    var submodelLocator = cardLocator.Locator("div.m-c-f1-pl--button span.ib").Nth(i);
                                    await submodelLocator.ClickAsync();
                                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                                }
                                string submodelName = item.TextContent.Replace("\n", " ").Trim();
                                submodelName = System.Text.RegularExpressions.Regex.Replace(submodelName, @"\s+", " ");
                                var packing = submodelName.EndsWith("OEM") ? "oem" : "box";
                                CreateAndAddCpu(cpus, uSpan, packing, socket, tdp, ram, image, card);
                            }
                        }
                        else
                        {
                            var nameTag = modelInfo.QuerySelector("span.u")?.TextContent + " " + modelInfo.QuerySelector("span.list-conf-name.ib.nobr")?.TextContent;
                            var packing = nameTag.EndsWith("OEM") ? "oem" : "box";
                            CreateAndAddCpu(cpus, uSpan, packing, socket, tdp, ram, image, card);
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
            csv.WriteRecords(cpus);
        }
        
        Console.WriteLine($"Successfully saved {cpus.Count} CPUs to '{FilePath}'.");
    }

    private static (string, int, string) GetModelDetails(IElement? detailsDiv)
    {
        string socket = "", ram = "";
        int tdp = 0;
        if (detailsDiv == null) return (socket, tdp, ram);

        foreach (var detail in detailsDiv.ChildNodes)
        {
            var text = detail.TextContent;
            if (text.Contains("Socket"))
                socket = detail.ChildNodes[1].TextContent;
            else if (text.Contains("TDP"))
                int.TryParse(detail.ChildNodes[1].TextContent.Replace("Вт", "").Trim(), out tdp);
            else if (text.Contains("ОЗП"))
                ram = detail.ChildNodes[1].TextContent;
        }
        return (socket, tdp, ram);
    }

    private static void CreateAndAddCpu(List<Cpu> list, string model, string? packing, string socket, int tdp, string ram, byte[] image, IElement card)
    {
        var priceInfo = card.QuerySelector("td.model-hot-prices-td");
        var (priceRange, offers) = BaseScraper.GetPriceInfo(priceInfo);

        var link = "https://ek.ua" + card.QuerySelector("div.model-short-links").QuerySelectorAll("a")
            .Where(n => n.TextContent.Contains("Ціни")).FirstOrDefault().GetAttribute("link");

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
            Image = image,
            PriceRange = priceRange,
            Offers = offers
        });
    }
}