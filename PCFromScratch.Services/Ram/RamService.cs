using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class RamService(IRamRepository ramRepository) : IRamService
{
    public async IAsyncEnumerable<RamDtoModel> GetRams(string? generation)
    {
        await foreach (var ram in ramRepository.GetRams(generation))
            yield return new(ram.Id, ram.Model, ram.Submodel, ram.Link, ram.Amount, ram.Sticks, ram.Voltage,
                ram.Generation, ram.Frequency, ram.ImageUrl, ram.MinPrice, ram.MaxPrice);
    }

    public async Task<RamDtoModel?> GetRam(Guid id)
    {
        var ram = await ramRepository.GetRam(id);
        if (ram is null) return null;

        return new(ram.Id, ram.Model, ram.Submodel, ram.Link, ram.Amount, ram.Sticks, ram.Voltage, ram.Generation,
            ram.Frequency, ram.ImageUrl, ram.MinPrice, ram.MaxPrice);
    }

    public async IAsyncEnumerable<OfferDtoModel> GetRamOffers(Guid id)
    {
        var ram = await ramRepository.GetRam(id);
        if (ram is not null)
            foreach (var offer in ram.Offers)
                yield return new(offer.ShopName, offer.Price, offer.City);
    }
}