using PCFromScratch.DBModels;
using PCFromScratch.Storage;

namespace PCFromScratch.Repository;

public class StorageRamRepository(IStorageContext storageContext) : IRamRepository
{
    private readonly IStorageContext _storageContext = storageContext;

    public IAsyncEnumerable<Ram> GetRams(string? generation = null) => _storageContext.GetRams(generation);

    public Task<Ram?> GetRam(Guid id) => _storageContext.GetRam(id);

    public Task AddRam(Ram ram) => _storageContext.AddRam(ram);

    public Task UpdateRam(Ram ram) => _storageContext.UpdateRam(ram);

    public Task RemoveRam(Guid id) => _storageContext.RemoveRam(id);
}