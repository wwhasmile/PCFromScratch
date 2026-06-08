using PCFromScratch.DBModels;
using PCFromScratch.Storage;

namespace PCFromScratch.Repository;

public class GpuBenchmarkRepository(IStorageContext storageContext) : IGpuBenchmarkRepository
{
    private readonly IStorageContext _storageContext = storageContext;

    public IAsyncEnumerable<GpuBenchmark> GetGpuBenchmarks(int? minScore)
        => _storageContext.GetGpuBenchmarks(minScore);

    public Task<GpuBenchmark?> GetGpuBenchmark(Guid id) => _storageContext.GetGpuBenchmark(id);

    public Task AddGpuBenchmark(GpuBenchmark gpuBenchmark) => _storageContext.AddGpuBenchmark(gpuBenchmark);

    public Task UpdateGpuBenchmark(GpuBenchmark gpuBenchmark) => _storageContext.UpdateGpuBenchmark(gpuBenchmark);

    public Task RemoveGpuBenchmark(Guid id) => _storageContext.RemoveGpuBenchmark(id);
}