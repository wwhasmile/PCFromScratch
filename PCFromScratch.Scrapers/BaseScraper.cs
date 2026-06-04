using System.Text.RegularExpressions;

using AngleSharp.Dom;

using Microsoft.Playwright;

using PCFromScratch.DBModels;

namespace PCFromScratch.Scrapers;

public class BaseScraper
{
    public static (string, List<Offer>) GetPriceInfo(IElement? priceInfo)
    {
        string priceRange = priceInfo.QuerySelector("div.model-price-range").TextContent.Split(".Порівняти")[0];
        List<Offer> offers = new List<Offer>();
        foreach (var offer in priceInfo.QuerySelector("table.model-hot-prices").QuerySelectorAll("tr")) //
        {
            var priceStr = offer.LastElementChild.TextContent;
            var offerPrice = Regex.Replace(priceStr, "[^0-9]", "");
            var offerShop = offer.FirstElementChild.QuerySelector("u").TextContent;
            var offerCity = offer.FirstElementChild.QuerySelector("nobr").TextContent;
            offers.Add(new Offer { ShopName = offerShop, Price = decimal.Parse(offerPrice), City = offerCity });
        }
        return (priceRange, offers);
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