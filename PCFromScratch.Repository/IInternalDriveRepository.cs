using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IInternalDriveRepository
{
    IAsyncEnumerable<InternalDrive> GetInternalDrives(string? type = null, int? capacity = null);

    Task<InternalDrive?> GetInternalDrive(Guid id);

    Task GetInternalDrive(InternalDrive internalDrive);
    Task UpdateInternalDrive(InternalDrive internalDrive);
    Task RemoveInternalDrive(Guid id);
}
