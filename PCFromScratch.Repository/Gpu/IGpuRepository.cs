using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IGpuRepository
{
    IAsyncEnumerable<Gpu> GetGpus();

    Task<Gpu?> GetGpu(Guid id);

    Task AddGpu(Gpu gpu);
    Task UpdateGpu(Gpu gpu);
    Task RemoveGpu(Guid id);
}
