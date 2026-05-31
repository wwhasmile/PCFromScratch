using PCFromScratch.DBModels;
using PCFromScratch.Storage;

namespace PCFromScratch.Repository;

public class StorageCoolerRepository(IStorageContext storageContext) : ICoolerRepository
{
    private readonly IStorageContext _storageContext = storageContext;

    public IAsyncEnumerable<Cooler> GetCoolers(string? socket = null) => _storageContext.GetCoolers(socket);

    public Task AddCooler(Cooler cooler) => _storageContext.AddCooler(cooler);

    public Task<Cooler?> GetCooler(Guid id) => _storageContext.GetCooler(id);

    public Task UpdateCooler(Cooler cooler) => _storageContext.UpdateCooler(cooler);

    public Task RemoveCooler(Guid id) => _storageContext.RemoveCooler(id);
}