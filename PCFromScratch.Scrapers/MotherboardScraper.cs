using System.Globalization;
using System.Text.RegularExpressions;

using AngleSharp;
using AngleSharp.Dom;
using CsvHelper;
using Microsoft.Playwright;
using PCFromScratch.DBModels;
using PCFromScratch.Common;

namespace PCFromScratch.Scrapers;

public class MotherboardScraper
{
    public static async Task<List<MotherboardRenamedForOmnissiah>> GetMotherboards()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });
        var page = await browser.NewPageAsync();

        var motherboards = new List<MotherboardRenamedForOmnissiah>();
        var random = new Random();

        var pageLink = "https://ek.ua/ua/list/187/";
        try
        {
            while (pageLink != null)
            {
                Console.WriteLine($"Retrieving page: {pageLink}");
                await page.GotoAsync(pageLink);

                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight / 2);");
                await Task.Delay(random.Next(1000, 2500));

                var content = await page.ContentAsync();
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(content));

                var cards = document.QuerySelectorAll("table.model-short-block");

                foreach (var card in cards)
                {
                    try
                    {
                        var priceInfo = card.QuerySelector("td.model-hot-prices-td");
                        var (minPr, maxPr, offers) = BaseScraper.GetPriceInfo(priceInfo);
                        
                        var modelInfo = card.QuerySelector("td.model-short-info");
                        var name = modelInfo.QuerySelector("span.u")?.TextContent;
                        if (string.IsNullOrEmpty(name)) continue;

                        var detailsDiv = modelInfo.QuerySelector("div.m-s-f2");
                        if (detailsDiv == null) continue;

                        var details = detailsDiv.ChildNodes;
                        
                        (string socket, MotherboardFormFactor formFactor, string chipset, string ramGeneration, int ramSlots, int ramFrequency, bool hasM2) = CheckDetails(details);
                        
                        var links = modelInfo.QuerySelector("div.model-short-links");
                        if(links == null) continue;
                        string link = "";
                        foreach (var linkInElement in links.QuerySelectorAll("a"))
                        {
                            var text = linkInElement.TextContent;
                            if (text.Contains("Ціни"))
                            {
                                link = "https://ek.ua" + linkInElement.GetAttribute("link");
                            }
                        }
                        if (link == "") continue;

                        var image = card.QuerySelector("img").GetAttribute("src");
                        
                        motherboards.Add(new MotherboardRenamedForOmnissiah
                        {
                            Id = Guid.NewGuid(),
                            Name = name,
                            Link = link,
                            Socket = socket,
                            FormFactor = formFactor,
                            Chipset = chipset,
                            RamGeneration = ramGeneration,
                            RamSlots = ramSlots,
                            RamFrequency = ramFrequency,
                            HasM2Slot = hasM2,
                            ImageUrl = image,
                            MaxPrice = maxPr,
                            MinPrice = minPr,
                            Offers = new HashSet<Offer>(offers)
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error scraping motherboard: {e.Message}");
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

        return motherboards;
    }

    private static (string, MotherboardFormFactor, string, string, int, int, bool) CheckDetails(INodeList details)
    {
        string socket = "", chipset = "", ramGeneration = "";
        MotherboardFormFactor formFactor = MotherboardFormFactor.ATX;
        int ramSlots = 0, ramFrequency = 0;
        bool hasM2 = false;

        foreach (var detail in details)
        {
            var text = Regex.Replace(detail.TextContent, "\\s", " ");
            if (text.Contains("Socket"))
                socket = detail.ChildNodes[1].TextContent.Trim();
            else if (text.Contains("Форм-фактор"))
                formFactor = detail.ChildNodes[1].TextContent.Trim() switch
                {
                    "ATX" => MotherboardFormFactor.ATX,
                    "micro-ATX" => MotherboardFormFactor.MicroATX,
                    "mini-ITX" => MotherboardFormFactor.MiniITX,
                    "EATX" => MotherboardFormFactor.EATX,
                    _ => MotherboardFormFactor.ATX
                };
            else if (text.Contains("Чипсет"))
                chipset = detail.ChildNodes[1].TextContent.Trim();
            else if (text.Contains("Слоти пам'яті"))
            {
                var slotStr = Regex.Match(text, @"\d+ х").Value;
                if (!string.IsNullOrEmpty(slotStr)) int.TryParse(slotStr.Replace("х", "").Trim(), out ramSlots);
                var gemStr = Regex.Match(text, "DDR\\d").Value;
                if (!string.IsNullOrEmpty(gemStr)) ramGeneration = gemStr;
                var freqStr = Regex.Match(text, @"\d+ МГц").Value;
                if (!string.IsNullOrEmpty(freqStr)) int.TryParse(freqStr.Replace("МГц", "").Trim(), out ramFrequency);
            }
            else if (text.Contains("Роз'єми"))
            {
                if (detail.ChildNodes[1].TextContent.Contains("M.2")) hasM2 = true;
            }
        }

        return (socket, formFactor, chipset, ramGeneration, ramSlots, ramFrequency, hasM2);
    }
}