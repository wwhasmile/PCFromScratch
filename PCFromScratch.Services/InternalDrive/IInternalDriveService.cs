using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

/// <summary>
/// Defines a service for interacting with Internal Drive data.
/// </summary>
public interface IInternalDriveService
{
    /// <summary>
    /// Retrieves a stream of internal drives, optionally filtered by type and capacity.
    /// </summary>
    /// <param name="type">Optional type to filter internal drives (e.g., HDD, SSD).</param>
    /// <param name="capacity">Optional minimum capacity to filter internal drives.</param>
    /// <returns>An asynchronous stream of internal drive models.</returns>
    IAsyncEnumerable<InternalDriveDtoModel> GetInternalDrives(string? type = null, int? capacity = null);

    /// <summary>
    /// Retrieves a specific internal drive by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the internal drive.</param>
    /// <returns>The internal drive model, or null if not found.</returns>
    Task<InternalDriveDtoModel?> GetInternalDrive(Guid id);

    /// <summary>
    /// Retrieves the available offers for a specific internal drive.
    /// </summary>
    /// <param name="id">The unique identifier of the internal drive.</param>
    /// <returns>An asynchronous stream of offer models.</returns>
    IAsyncEnumerable<OfferDtoModel> GetInternalDriveOffers(Guid id);
}