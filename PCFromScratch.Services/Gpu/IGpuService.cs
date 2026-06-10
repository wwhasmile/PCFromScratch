using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface IGpuService
{
    IAsyncEnumerable<GpuDtoModel> GetGpus();

    Task<GpuDtoModel?> GetGpu(Guid id);

    IAsyncEnumerable<OfferDtoModel> GetGpuOffers(Guid id);
}