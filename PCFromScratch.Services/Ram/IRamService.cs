using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface IRamService
{
    IAsyncEnumerable<RamDtoModel> GetRams(string? generation = null);

    Task<RamDtoModel?> GetRam(Guid id);

    IAsyncEnumerable<RamDtoModel> GetRamOffers(Guid id);
}