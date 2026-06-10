using PCFromScratch.Common;
using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class PsuService(IPsuRepository psuRepository) : IPsuService
{
    public async IAsyncEnumerable<PsuDtoModel> GetPsus(int? minPower = null)
    {
        await foreach (var psu in psuRepository.GetPsus(minPower))
            yield return new(psu.Id, psu.Name, psu.Link, psu.Power, psu.Level, psu.PowerConnector, psu.FormFactor,
                psu.Modularity, psu.ImageUrl, psu.MinPrice, psu.MaxPrice);
    }

    public async Task<PsuDtoModel?> GetPsu(Guid id)
    {
        var psu = await psuRepository.GetPsu(id);
        if (psu is null) return null;

        return new(psu.Id, psu.Name, psu.Link, psu.Power, psu.Level, psu.PowerConnector, psu.FormFactor, psu.Modularity,
            psu.ImageUrl, psu.MinPrice, psu.MaxPrice);
    }

    public async IAsyncEnumerable<OfferDtoModel> GetPsuOffers(Guid id)
    {
        var psu = await psuRepository.GetPsu(id);
        if (psu is not null)
            foreach (var offer in psu.Offers)
                yield return new(offer.ShopName, offer.Price, offer.City);
    }
}