using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface IInternalDriveService
{
    IAsyncEnumerable<InternalDriveDtoModel> GetInternalDrives(string? type = null, int? capacity = null);

    Task<InternalDriveDtoModel?> GetInternalDrive(Guid id);

    IAsyncEnumerable<OfferDtoModel> GetInternalDriveOffers(Guid id);
}