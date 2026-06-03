using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp;
using CsvHelper;
using Microsoft.Playwright;

using PCFromScratch.DBModels;

namespace PCFromScratch.Scrapers;

public class GpuScraper
{
    //I cannot test some lines until db is ready, so I will mark them with empty comments
    private static readonly string FilePath = "data/gpus.csv";
    public static async Task GetGpus()
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
            Headless = false // Set to false to see the browser UI
        });
        var page = await browser.NewPageAsync();

        var gpus = new List<Gpu>();
        var random = new Random();

        var pageLink = "https://ek.ua/ua/ek-list.php?katalog_=189&order_=param_desc&op_=8161";
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
                await Task.Delay(random.Next(1000, 5000));

                var content = await page.ContentAsync();
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(content));

                var cards = document.QuerySelectorAll("table.model-short-block");

                foreach (var card in cards)
                {
                    try
                    {
                        var priceInfo = card.QuerySelector("td.model-hot-prices-td"); //
                        var priceRange = priceInfo.QuerySelector("div. model-price-range").TextContent; //
                        List<Offer> offers = new List<Offer>();
                        foreach (var offer in priceInfo.QuerySelector("table.model-hot-prices").QuerySelectorAll("tr")) //
                        {
                            var offerLink = offer.LastElementChild.GetAttribute("href");
                            var priceStr = offer.LastElementChild.TextContent;
                            var offerPrice = Regex.Replace(priceStr, "[^0-9]", "");
                            var offerShop = offer.FirstElementChild.QuerySelector("u").TextContent;
                            var offerCity = offer.FirstElementChild.QuerySelector("nobr").TextContent;
                            offers.Add(new Offer { ShopName = offerShop, Link = offerLink, Price = decimal.Parse(offerPrice), City = offerCity });
                        }
                        
                        var modelInfo = card.QuerySelector("td.model-short-info");
                        var name = modelInfo.QuerySelector("span.u")?.TextContent;
                        if (string.IsNullOrEmpty(name)) continue;

                        Task<byte[]> imageTask = page.GetByAltText($"Відеокарта {name}").ScreenshotAsync(); //

                        var detailsDiv = modelInfo.QuerySelector("div.m-s-f2");
                        if (detailsDiv == null) continue;

                        var details = detailsDiv.ChildNodes;
                        int tdp = 0;
                        int length = 0;

                        foreach (var detail in details)
                        {
                            var text = detail.TextContent;
                            if (text.Contains("Живлення") && text.Contains("Вт"))
                            {
                                var parts = text.Replace("Вт", "").Trim().Split(' ');
                                tdp = int.Parse(parts.Last().Trim());
                            }

                            if (text.Contains("Довжина"))
                            {
                                length = int.Parse(text.Replace("мм", "").Trim().Split(":").Last());
                            }
                        }
                        
                        var links = modelInfo.QuerySelector("div.model-short-links"); //
                        if(links == null) continue;
                        string link = "";
                        foreach (var linkInElement in links.QuerySelectorAll("a")) //
                        {
                            var text = linkInElement.TextContent;
                            if (text.Contains("Ціни"))
                            {
                                link = "https://ek.ua" + linkInElement.GetAttribute("link"); //
                            }
                        }
                        if (tdp==0 || length==0) continue;
                        
                        var image = await imageTask; //
                        
                        gpus.Add(new Gpu
                        {
                            Name = name,
                            Link = link,
                            Tdp = tdp,
                            Length = length,
                            Image = image,
                            PriceRange = priceRange,
                            Offers = offers
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"No data about GPU: {e.Message}, returning current result");
                        goto EndScraping; // Exit loops and proceed to file writing
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

        EndScraping:; // Label for goto

        using (var writer = new StreamWriter("data/gpus.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(gpus);
        }
        
        Console.WriteLine($"Successfully saved {gpus.Count} GPUs to 'data/gpus.csv'.");
    }
}
