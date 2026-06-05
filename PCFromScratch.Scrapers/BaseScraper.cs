using System.Text.RegularExpressions;

using AngleSharp.Dom;

using Microsoft.Playwright;

using PCFromScratch.DBModels;

namespace PCFromScratch.Scrapers;

public class BaseScraper
{
    public static (int, int, HashSet<Offer>) GetPriceInfo(IElement? priceInfo)
    {
        string priceRange = priceInfo.QuerySelector("div.model-price-range").TextContent.Split(".Порівняти")[0];
        var minMaxPrice = priceRange.Split("до");
        int min = 0, max = 0;
        if (minMaxPrice.Length > 1)
        {
            min = int.Parse(Regex.Replace(minMaxPrice[0], "^[0-9]", ""));
            max = int.Parse(Regex.Replace(minMaxPrice[1], "[^0-9]", ""));
        }
        else
        {
            min = int.Parse(Regex.Replace(minMaxPrice[0], "^[0-9]", ""));
            max = min;
        }
        HashSet<Offer> offers = new HashSet<Offer>();
        foreach (var offer in priceInfo.QuerySelector("table.model-hot-prices").QuerySelectorAll("tr")) //
        {
            var priceStr = offer.LastElementChild.TextContent;
            var offerPrice = Regex.Replace(priceStr, "[^0-9]", "");
            var offerShop = offer.FirstElementChild.QuerySelector("u").TextContent;
            var offerCity = offer.FirstElementChild.QuerySelector("nobr").TextContent;
            offers.Add(new Offer { ShopName = offerShop, Price = decimal.Parse(offerPrice), City = offerCity });
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