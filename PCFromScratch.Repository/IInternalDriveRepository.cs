using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IInternalDriveRepository
{
    IAsyncEnumerable<Ram> GetInternalDrives(string? type, int? capacity);

    Task<Ram> GetInternalDrive(Guid id);

    Task GetInternalDrive(InternalDrive internalDrive);
    Task UpdateInternalDrive(InternalDrive internalDrive);
    Task RemoveInternalDrive(Guid id);
}
