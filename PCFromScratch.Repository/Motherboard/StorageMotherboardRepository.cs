using PCFromScratch.DBModels;
using PCFromScratch.Storage;

namespace PCFromScratch.Repository;

public class StorageMotherboardRepository(IStorageContext storageContext) : IMotherboardRepository
{
    private readonly IStorageContext _storageContext = storageContext;

    public IAsyncEnumerable<MotherboardRenamedForOmnissiah> GetMotherboards(string? socket = null)
        => _storageContext.GetMotherboards(socket);

    public Task<MotherboardRenamedForOmnissiah?> GetMotherboard(Guid id) => _storageContext.GetMotherboard(id);

    public Task AddMotherboard(MotherboardRenamedForOmnissiah motherboardRenamedForOmnissiah) => _storageContext.AddMotherboard(motherboardRenamedForOmnissiah);

    public Task UpdateMotherboard(MotherboardRenamedForOmnissiah motherboardRenamedForOmnissiah) => _storageContext.UpdateMotherboard(motherboardRenamedForOmnissiah);

    public Task RemoveMotherboard(Guid id) => _storageContext.RemoveMotherboard(id);
}