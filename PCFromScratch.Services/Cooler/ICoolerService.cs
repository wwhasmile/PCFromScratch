using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface ICoolerService
{
    IAsyncEnumerable<CoolerDtoModel> GetCoolers(string? socket = null);

    Task<CoolerDtoModel?> GetCooler(Guid id);

    IAsyncEnumerable<OfferDtoModel> GetCoolerOffers(Guid id);
}