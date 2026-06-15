using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

/// <summary>
/// Defines a service for interacting with CPU Benchmark data.
/// </summary>
public interface ICpuBenchmarkService
{
    /// <summary>
    /// Retrieves a stream of CPU benchmarks, optionally filtered by a minimum score.
    /// </summary>
    /// <param name="minScore">Optional minimum score to filter benchmarks.</param>
    /// <returns>An asynchronous stream of CPU benchmark models.</returns>
    IAsyncEnumerable<CpuBenchmarkDtoModel> GetCpuBenchmarks(int? minScore = null);

    /// <summary>
    /// Retrieves a specific CPU benchmark by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the CPU benchmark.</param>
    /// <returns>The CPU benchmark model, or null if not found.</returns>
    Task<CpuBenchmarkDtoModel?> GetCpuBenchmark(Guid id);

    /// <summary>
    /// Retrieves a specific CPU benchmark by the name of the CPU.
    /// </summary>
    /// <param name="name">The name of the CPU.</param>
    /// <returns>The CPU benchmark model, or null if not found.</returns>
    Task<CpuBenchmarkDtoModel?> GetCpuBenchmark(string name);
}