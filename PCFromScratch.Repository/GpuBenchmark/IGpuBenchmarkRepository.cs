using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IGpuBenchmarkRepository
{
    IAsyncEnumerable<GpuBenchmark> GetGpuBenchmarks(int? minScore = null);

    Task<GpuBenchmark?> GetGpuBenchmark(Guid id);

    Task AddGpuBenchmark(GpuBenchmark gpuBenchmark);
    Task UpdateGpuBenchmark(GpuBenchmark gpuBenchmark);
    Task RemoveGpuBenchmark(Guid id);
}
