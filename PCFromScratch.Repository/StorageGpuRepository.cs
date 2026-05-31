using PCFromScratch.DBModels;
using PCFromScratch.Storage;

namespace PCFromScratch.Repository;

public class StorageGpuRepository(IStorageContext storageContext) : IGpuRepository
{
    private readonly IStorageContext _storageContext = storageContext;

    public IAsyncEnumerable<Gpu> GetGpus() => _storageContext.GetGpus();

    public Task<Gpu?> GetGpu(Guid id) => _storageContext.GetGpu(id);

    public Task AddGpu(Gpu gpu) => _storageContext.AddGpu(gpu);

    public Task UpdateGpu(Gpu gpu) => _storageContext.UpdateGpu(gpu);

    public Task RemoveGpu(Guid id) => _storageContext.RemoveGpu(id);
}