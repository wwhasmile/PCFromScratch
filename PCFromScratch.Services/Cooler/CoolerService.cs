using PCFromScratch.Common;
using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class CoolerService(ICoolerRepository coolerRepository) : ICoolerService
{
    public async IAsyncEnumerable<CoolerDtoModel> GetCoolers(string? socket = null)
    {
        await foreach (var cooler in coolerRepository.GetCoolers(socket))
            yield return new(cooler.Id, cooler.Name, cooler.Tdp, cooler.FanCount, cooler.Radius, cooler.Thickness,
                cooler.Speed, cooler.Height, cooler.Type, cooler.ImageUrl, cooler.MinPrice, cooler.MaxPrice);
    }

    public async Task<CoolerDtoModel?> GetCooler(Guid id)
    {
        var cooler = await coolerRepository.GetCooler(id);
        if (cooler is null) return null;

        return new(cooler.Id, cooler.Name, cooler.Tdp, cooler.FanCount, cooler.Radius, cooler.Thickness, cooler.Speed,
            cooler.Height, cooler.Type, cooler.ImageUrl, cooler.MinPrice, cooler.MaxPrice);
    }

    public async IAsyncEnumerable<OfferDtoModel> GetCoolerOffers(Guid id)
    {
        var cooler = await coolerRepository.GetCooler(id);
        if (cooler is not null)
            foreach (var offer in cooler.Offers)
                yield return new(offer.ShopName, offer.Price, offer.City);
    }
}