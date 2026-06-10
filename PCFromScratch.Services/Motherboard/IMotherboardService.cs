using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface IMotherboardService
{
    IAsyncEnumerable<MotherboardDtoModel> GetMotherboards(string? socket = null);

    Task<MotherboardDtoModel?> GetMotherboard(Guid id);

    IAsyncEnumerable<OfferDtoModel> GetMotherboardOffers(Guid id);
}