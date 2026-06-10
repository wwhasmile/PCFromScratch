using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class MotherboardService(IMotherboardRepository motherboardRepository) : IMotherboardService
{
    public async IAsyncEnumerable<MotherboardDtoModel> GetMotherboards(string? socket = null)
    {
        await foreach (var motherboard in motherboardRepository.GetMotherboards(socket))
            yield return new(motherboard.Id, motherboard.Name, motherboard.Socket, motherboard.FormFactor,
                motherboard.Chipset, motherboard.RamGeneration, motherboard.RamSlots, motherboard.RamFrequency,
                motherboard.HasM2Slot, motherboard.ImageUrl, motherboard.MinPrice, motherboard.MaxPrice);
    }

    public async Task<MotherboardDtoModel?> GetMotherboard(Guid id)
    {
        var motherboard = await motherboardRepository.GetMotherboard(id);
        if (motherboard is null) return null;

        return new(motherboard.Id, motherboard.Name, motherboard.Socket, motherboard.FormFactor, motherboard.Chipset,
            motherboard.RamGeneration, motherboard.RamSlots, motherboard.RamFrequency, motherboard.HasM2Slot,
            motherboard.ImageUrl, motherboard.MinPrice, motherboard.MaxPrice);
    }

    public async IAsyncEnumerable<OfferDtoModel> GetMotherboardOffers(Guid id)
    {
        var motherboard = await motherboardRepository.GetMotherboard(id);
        if (motherboard is not null)
            foreach (var offer in motherboard.Offers)
                yield return new(offer.ShopName, offer.Price, offer.City);
    }
}