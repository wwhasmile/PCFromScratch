using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IInternalDriveRepository
{
    IAsyncEnumerable<Ram> GetInternalDrives(string? type = null, int? capacity = null);

    Task<Ram> GetInternalDrive(Guid id);

    Task GetInternalDrive(InternalDrive internalDrive);
    Task UpdateInternalDrive(InternalDrive internalDrive);
    Task RemoveInternalDrive(Guid id);
}
