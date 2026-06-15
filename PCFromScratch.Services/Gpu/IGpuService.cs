using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

/// <summary>
/// Defines a service for interacting with GPU data.
/// </summary>
public interface IGpuService
{
    /// <summary>
    /// Retrieves a stream of all available GPUs.
    /// </summary>
    /// <returns>An asynchronous stream of GPU models.</returns>
    IAsyncEnumerable<GpuDtoModel> GetGpus();

    /// <summary>
    /// Retrieves a specific GPU by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the GPU.</param>
    /// <returns>The GPU model, or null if not found.</returns>
    Task<GpuDtoModel?> GetGpu(Guid id);

    /// <summary>
    /// Retrieves the available offers for a specific GPU.
    /// </summary>
    /// <param name="id">The unique identifier of the GPU.</param>
    /// <returns>An asynchronous stream of offer models.</returns>
    IAsyncEnumerable<OfferDtoModel> GetGpuOffers(Guid id);
}