using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface IPsuService
{
    IAsyncEnumerable<PsuDtoModel> GetPsus(int? minPower = null);

    Task<PsuDtoModel?> GetPsu(Guid id);

    IAsyncEnumerable<OfferDtoModel> GetPsuOffers(Guid id);
}