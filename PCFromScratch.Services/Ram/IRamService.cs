using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

/// <summary>
/// Defines a service for interacting with RAM (Random Access Memory) data.
/// </summary>
public interface IRamService
{
    /// <summary>
    /// Retrieves a stream of RAM modules, optionally filtered by generation.
    /// </summary>
    /// <param name="generation">Optional generation to filter RAM modules.</param>
    /// <returns>An asynchronous stream of RAM models.</returns>
    IAsyncEnumerable<RamDtoModel> GetRams(string? generation = null);

    /// <summary>
    /// Retrieves a specific RAM module by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the RAM module.</param>
    /// <returns>The RAM model, or null if not found.</returns>
    Task<RamDtoModel?> GetRam(Guid id);

    /// <summary>
    /// Retrieves the available offers for a specific RAM module.
    /// </summary>
    /// <param name="id">The unique identifier of the RAM module.</param>
    /// <returns>An asynchronous stream of offer models.</returns>
    IAsyncEnumerable<OfferDtoModel> GetRamOffers(Guid id);
}