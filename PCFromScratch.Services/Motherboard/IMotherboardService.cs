using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

/// <summary>
/// Defines a service for interacting with Motherboard data.
/// </summary>
public interface IMotherboardService
{
    /// <summary>
    /// Retrieves a stream of motherboards, optionally filtered by socket type.
    /// </summary>
    /// <param name="socket">Optional socket type to filter motherboards.</param>
    /// <returns>An asynchronous stream of motherboard models.</returns>
    IAsyncEnumerable<MotherboardDtoModel> GetMotherboards(string? socket = null);

    /// <summary>
    /// Retrieves a specific motherboard by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the motherboard.</param>
    /// <returns>The motherboard model, or null if not found.</returns>
    Task<MotherboardDtoModel?> GetMotherboard(Guid id);

    /// <summary>
    /// Retrieves the available offers for a specific motherboard.
    /// </summary>
    /// <param name="id">The unique identifier of the motherboard.</param>
    /// <returns>An asynchronous stream of offer models.</returns>
    IAsyncEnumerable<OfferDtoModel> GetMotherboardOffers(Guid id);
}