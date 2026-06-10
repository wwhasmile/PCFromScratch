using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class InternalDriveService(IInternalDriveRepository internalDriveRepository) : IInternalDriveService
{
    public async IAsyncEnumerable<InternalDriveDtoModel> GetInternalDrives(string? type = null, int? capacity = null)
    {
        await foreach (var internalDrive in internalDriveRepository.GetInternalDrives(type, capacity))
            yield return new(internalDrive.Id, internalDrive.Name, internalDrive.Capacity, internalDrive.Type,
                internalDrive.Port, internalDrive.ImageUrl, internalDrive.MinPrice, internalDrive.MaxPrice);
    }

    public async Task<InternalDriveDtoModel?> GetInternalDrive(Guid id)
    {
        var internalDrive = await internalDriveRepository.GetInternalDrive(id);
        if (internalDrive is null) return null;

        return new(internalDrive.Id, internalDrive.Name, internalDrive.Capacity, internalDrive.Type,
            internalDrive.Port, internalDrive.ImageUrl, internalDrive.MinPrice, internalDrive.MaxPrice);
    }

    public async IAsyncEnumerable<OfferDtoModel> GetInternalDriveOffers(Guid id)
    {
        var internalDrive = await internalDriveRepository.GetInternalDrive(id);
        if (internalDrive is not null)
            foreach (var offer in internalDrive.Offers)
                yield return new(offer.ShopName, offer.Price, offer.City);
    }
}