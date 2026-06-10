using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface ICpuService
{
    IAsyncEnumerable<CpuDtoModel> GetCpus(string? socket = null);

    Task<CpuDtoModel?> GetCpu(Guid id);

    IAsyncEnumerable<OfferDtoModel> GetCpuOffers(Guid id);
}