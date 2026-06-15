using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

/// <summary>
/// Defines a service for interacting with Cooler data.
/// </summary>
public interface ICoolerService
{
    /// <summary>
    /// Retrieves a stream of coolers, optionally filtered by socket type.
    /// </summary>
    /// <param name="socket">Optional socket type to filter coolers.</param>
    /// <returns>An asynchronous stream of cooler models.</returns>
    IAsyncEnumerable<CoolerDtoModel> GetCoolers(string? socket = null);

    /// <summary>
    /// Retrieves a specific cooler by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the cooler.</param>
    /// <returns>The cooler model, or null if not found.</returns>
    Task<CoolerDtoModel?> GetCooler(Guid id);

    /// <summary>
    /// Retrieves the available offers for a specific cooler.
    /// </summary>
    /// <param name="id">The unique identifier of the cooler.</param>
    /// <returns>An asynchronous stream of offer models.</returns>
    IAsyncEnumerable<OfferDtoModel> GetCoolerOffers(Guid id);
}