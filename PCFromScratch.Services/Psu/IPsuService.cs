using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

/// <summary>
/// Defines a service for interacting with PSU (Power Supply Unit) data.
/// </summary>
public interface IPsuService
{
    /// <summary>
    /// Retrieves a stream of PSUs, optionally filtered by minimum power.
    /// </summary>
    /// <param name="minPower">Optional minimum power to filter PSUs.</param>
    /// <returns>An asynchronous stream of PSU models.</returns>
    IAsyncEnumerable<PsuDtoModel> GetPsus(int? minPower = null);

    /// <summary>
    /// Retrieves a specific PSU by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the PSU.</param>
    /// <returns>The PSU model, or null if not found.</returns>
    Task<PsuDtoModel?> GetPsu(Guid id);

    /// <summary>
    /// Retrieves the available offers for a specific PSU.
    /// </summary>
    /// <param name="id">The unique identifier of the PSU.</param>
    /// <returns>An asynchronous stream of offer models.</returns>
    IAsyncEnumerable<OfferDtoModel> GetPsuOffers(Guid id);
}