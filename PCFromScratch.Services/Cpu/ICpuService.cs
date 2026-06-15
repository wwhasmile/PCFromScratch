using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

/// <summary>
/// Defines a service for interacting with CPU data.
/// </summary>
public interface ICpuService
{
    /// <summary>
    /// Retrieves a stream of CPUs, optionally filtered by socket type.
    /// </summary>
    /// <param name="socket">Optional socket type to filter CPUs.</param>
    /// <returns>An asynchronous stream of CPU models.</returns>
    IAsyncEnumerable<CpuDtoModel> GetCpus(string? socket = null);

    /// <summary>
    /// Retrieves a specific CPU by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the CPU.</param>
    /// <returns>The CPU model, or null if not found.</returns>
    Task<CpuDtoModel?> GetCpu(Guid id);

    /// <summary>
    /// Retrieves the available offers for a specific CPU.
    /// </summary>
    /// <param name="id">The unique identifier of the CPU.</param>
    /// <returns>An asynchronous stream of offer models.</returns>
    IAsyncEnumerable<OfferDtoModel> GetCpuOffers(Guid id);
}