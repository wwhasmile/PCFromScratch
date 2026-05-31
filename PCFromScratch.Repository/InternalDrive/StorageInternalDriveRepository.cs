using PCFromScratch.DBModels;
using PCFromScratch.Storage;

namespace PCFromScratch.Repository;

public class StorageInternalDriveRepository(IStorageContext storageContext) : IInternalDriveRepository
{
    private readonly IStorageContext _storageContext = storageContext;

    public IAsyncEnumerable<InternalDrive> GetInternalDrives(string? type, int? capacity)
    => _storageContext.GetInternalDrives(type, capacity);

    public Task<InternalDrive?> GetInternalDrive(Guid id) => _storageContext.GetInternalDrive(id);

    public Task AddInternalDrive(InternalDrive internalDrive) => _storageContext.AddInternalDrive(internalDrive);

    public Task UpdateInternalDrive(InternalDrive internalDrive) => _storageContext.UpdateInternalDrive(internalDrive);

    public Task RemoveInternalDrive(Guid id) => _storageContext.RemoveInternalDrive(id);
}