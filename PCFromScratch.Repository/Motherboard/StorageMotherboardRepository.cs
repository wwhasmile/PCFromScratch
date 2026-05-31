using PCFromScratch.DBModels;
using PCFromScratch.Storage;

namespace PCFromScratch.Repository;

public class StorageMotherboardRepository(IStorageContext storageContext) : IMotherboardRepository
{
    private readonly IStorageContext _storageContext = storageContext;

    public IAsyncEnumerable<Motherboard> GetMotherboards(string? socket = null)
        => _storageContext.GetMotherboards(socket);

    public Task<Motherboard?> GetMotherboard(Guid id) => _storageContext.GetMotherboard(id);

    public Task AddMotherboard(Motherboard motherboard) => _storageContext.AddMotherboard(motherboard);

    public Task UpdateMotherboard(Motherboard motherboard) => _storageContext.UpdateMotherboard(motherboard);

    public Task RemoveMotherboard(Guid id) => _storageContext.RemoveMotherboard(id);
}