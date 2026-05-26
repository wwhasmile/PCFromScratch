using System.Globalization;
using AngleSharp;
using CsvHelper;
using Microsoft.Playwright;

using PCFromScratch.Scrapers.CSVModels;

namespace PCFromScratch.Scrapers;

//For now install Playwright browsers by writing in code Microsoft.Playwright.Program.Main(new[] { "install" }); during first launch, then I'm planning to add browsers in release app
public class CpuScraper
{
    private static readonly string FilePath = "data/cpus.csv";
    public static async Task GetCpusForSale()
    {
        if (!File.Exists(FilePath))
        {
            string? directoryName = Path.GetDirectoryName(FilePath);
            if (directoryName is not null && !Directory.Exists(Path.GetDirectoryName(FilePath)))
            {
                Directory.CreateDirectory(directoryName);
            }
            File.Create(FilePath).Close();
        }
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true // Set to false to see the browser UI
        });
        var page = await browser.NewPageAsync();

        var cpus = new List<Cpu>();
        var random = new Random();

        var pageLink = "https://ek.ua/ua/list/186/";
        try
        {
            while (pageLink != null)
            {
                Console.WriteLine($"Retrieving page: {pageLink}");
                await page.GotoAsync(pageLink);

                // Simulate Human Behavior
                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight / 2);");
                await Task.Delay(random.Next(1000, 2500));
                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight);");
                await Task.Delay(random.Next(4500, 8500));

                var content = await page.ContentAsync();
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(content));

                var cards = document.QuerySelectorAll("td.model-short-info");

                foreach (var card in cards)
                {
                    try
                    {
                        var nameTag = card.QuerySelector("span.u")?.TextContent + " " + card.QuerySelector("span.list-conf-name.ib.nobr")?.TextContent;
                        if (string.IsNullOrEmpty(nameTag)) continue;

                        var boxOem = nameTag.EndsWith("BOX") ? "box" : "oem";
                        var name = nameTag.Substring(0, nameTag.Length - 4);
                        var link = "https://ek.ua" + card.QuerySelector("a.model-short-title.no-u")?.GetAttribute("href");

                        var detailsDiv = card.QuerySelector("div.m-s-f2");
                        if (detailsDiv == null) continue;

                        var details = detailsDiv.ChildNodes;
                        string tdp = "", socket = "", ram = "";

                        foreach (var detail in details)
                        {
                            if (detail.TextContent.Contains("Socket"))
                                socket = detail.ChildNodes[1].TextContent;
                            else if (detail.TextContent.Contains("TDP"))
                                tdp = detail.ChildNodes[1].TextContent.Replace("Вт", "").Trim();
                            else if (detail.TextContent.Contains("ОЗП"))
                                ram = detail.ChildNodes[1].TextContent;
                        }

                        if (string.IsNullOrEmpty(socket) || string.IsNullOrEmpty(tdp) || string.IsNullOrEmpty(ram)) continue;

                        var ramSplitted = ram.Split(", ");
                        var ramMax = ram.StartsWith("макс") ? ram.Split(" ")[1].Replace("ГБ", "") : "";
                        if (ramMax.Length > 0)
                        {
                            ramMax = ramMax.Substring(0, ramMax.Length - 2);
                        }
                        string ramGen = "", ramFreq = "";

                        if (ramSplitted.Length > 1 || !ram.StartsWith("макс"))
                        {
                            var ramInfo = ramSplitted.Last().Replace("МГц", "").Trim().Split(" ");
                            ramGen = ramInfo[0];
                            ramFreq = ramInfo[1];
                        }

                        cpus.Add(new Cpu
                        {
                            Name = name,
                            Link = link,
                            Socket = socket,
                            Tdp = tdp,
                            RamMax = ramMax,
                            RamGen = ramGen,
                            RamFreq = ramFreq,
                            BoxOem = boxOem
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Skipping a malformed card. Error: {e.Message}");
                        continue;
                    }
                }

                var nextButton = document.QuerySelector("a.ib.pager-next");
                pageLink = nextButton != null ? "https://ek.ua" + nextButton.GetAttribute("href") : null;
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
    }
}
