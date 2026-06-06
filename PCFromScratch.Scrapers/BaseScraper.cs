using System.Text.RegularExpressions;

using AngleSharp.Dom;

using Microsoft.Playwright;

using PCFromScratch.DBModels;

namespace PCFromScratch.Scrapers;

public class BaseScraper
{
    public static (int, int, HashSet<Offer>) GetPriceInfo(IElement? priceInfo)
    {
        int min = 0, max = 0;
        HashSet<Offer> offers = new HashSet<Offer>();
        var range = priceInfo.QuerySelector("div.model-price-range");
        if (range != null)
        {
            string priceRange = range.TextContent.Split(".Порівняти")[0];
            var minMaxPrice = priceRange.Split("до");
            min = int.Parse(Regex.Replace(minMaxPrice[0], "[^0-9]", ""));
            max = int.Parse(Regex.Replace(minMaxPrice[1], "[^0-9]", ""));
            foreach (var offer in priceInfo.QuerySelector("table.model-hot-prices").QuerySelectorAll("tr")) //
            {
                var priceStr = offer.LastElementChild.TextContent;
                var offerPrice = Regex.Replace(priceStr, "[^0-9]", "");
                var offerShop = offer.FirstElementChild.QuerySelector("u").TextContent;
                var offerCity = offer.FirstElementChild.QuerySelector("nobr").TextContent;
                offers.Add(new Offer { Id = Guid.NewGuid(), ShopName = offerShop, Price = decimal.Parse(offerPrice), City = offerCity });
            }
        }
        else
        {
            string price = priceInfo.QuerySelector("div.pr31").TextContent;
            min = int.Parse(Regex.Replace(price, "[^0-9]", ""));
            max = min;
            var sellerCard = priceInfo.QuerySelector("div.pr31-sh");
            var sellerMarketplace = sellerCard.QuerySelector("div.seller");
            string seller;
            if (sellerMarketplace != null)
            {
                seller = sellerMarketplace.TextContent;
            }
            else
            {
                seller = sellerCard.TextContent;
            }
            offers.Add(new Offer { Id = Guid.NewGuid(), ShopName = seller, Price = min, City = "Невідомо" });
        }
        return (min, max, offers);
    }   
    public static async Task<(int, int, HashSet<Offer>)> GetPriceInfoAsync(ILocator priceInfo)
    {
        int min = 0, max = 0;
        HashSet<Offer> offers = new HashSet<Offer>();
        var range = priceInfo.Locator("div.model-price-range");
        if (await range.CountAsync() > 0)
        {
            string priceRangeText = await range.TextContentAsync();
            string priceRange = priceRangeText.Split(".Порівняти")[0];
            var minMaxPrice = priceRange.Split("до");
            min = int.Parse(Regex.Replace(minMaxPrice[0], "[^0-9]", ""));
            max = int.Parse(Regex.Replace(minMaxPrice[1], "[^0-9]", ""));
            var rows = priceInfo.Locator("table.model-hot-prices tr");
            var count = await rows.CountAsync();
            for (int i = 0; i < count; i++)
            {
                var row = rows.Nth(i);
                var priceStr = await row.Locator("td").Last.TextContentAsync();
                var offerPrice = Regex.Replace(priceStr, "[^0-9]", "");
                var offerShop = await row.Locator("td").First.Locator("u").TextContentAsync();
                var offerCity = await row.Locator("td").First.Locator("nobr").TextContentAsync();
                offers.Add(new Offer { Id = Guid.NewGuid(), ShopName = offerShop, Price = decimal.Parse(offerPrice), City = offerCity });
            }
        }
        else
        {
            string price = await priceInfo.Locator("div.pr31").TextContentAsync();
            min = int.Parse(Regex.Replace(price, "[^0-9]", ""));
            max = min;
            var sellerCard = priceInfo.Locator("div.pr31-sh");
            var sellerMarketplace = sellerCard.Locator("div.seller");
            string seller;
            if (await sellerMarketplace.CountAsync() > 0)
            {
                seller = await sellerMarketplace.TextContentAsync() ?? "Невідомо";
            }
            else
            {
                seller = await sellerCard.TextContentAsync() ?? "Невідомо";
            }
            offers.Add(new Offer { Id = Guid.NewGuid(), ShopName = seller, Price = min, City = "Невідомо" });
        }
        return (min, max, offers);
    }


    public static void CreatePath(string filePath)
    {
        if (!File.Exists(filePath))
        {
            string? directoryName = Path.GetDirectoryName(filePath);
            if (directoryName is not null && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            File.Create(filePath).Close();
        }
    }
}