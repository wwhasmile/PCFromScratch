using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

/// <summary>
/// Defines a service for interacting with GPU Benchmark data.
/// </summary>
public interface IGpuBenchmarkService
{
    /// <summary>
    /// Retrieves a stream of GPU benchmarks, optionally filtered by a minimum score.
    /// </summary>
    /// <param name="minScore">Optional minimum score to filter benchmarks.</param>
    /// <returns>An asynchronous stream of GPU benchmark models.</returns>
    IAsyncEnumerable<GpuBenchmarkDtoModel> GetGpuBenchmarks(int? minScore = null);

    /// <summary>
    /// Retrieves a specific GPU benchmark by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the GPU benchmark.</param>
    /// <returns>The GPU benchmark model, or null if not found.</returns>
    Task<GpuBenchmarkDtoModel?> GetGpuBenchmark(Guid id);

    /// <summary>
    /// Retrieves a specific GPU benchmark by the name of the GPU.
    /// </summary>
    /// <param name="name">The name of the GPU.</param>
    /// <returns>The GPU benchmark model, or null if not found.</returns>
    Task<GpuBenchmarkDtoModel?> GetGpuBenchmark(string name);
}