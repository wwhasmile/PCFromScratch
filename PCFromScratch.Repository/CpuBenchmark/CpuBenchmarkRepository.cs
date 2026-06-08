using PCFromScratch.DBModels;
using PCFromScratch.Storage;

namespace PCFromScratch.Repository;

public class CpuBenchmarkRepository(IStorageContext storageContext) : ICpuBenchmarkRepository
{
    private readonly IStorageContext _storageContext = storageContext;

    public IAsyncEnumerable<CpuBenchmark> GetCpuBenchmarks(int? minScore)
        => _storageContext.GetCpuBenchmarks(minScore);

    public Task<CpuBenchmark?> GetCpuBenchmark(Guid id) => _storageContext.GetCpuBenchmark(id);

    public Task<CpuBenchmark?> GetCpuBenchmark(string cpuName) => _storageContext.GetCpuBenchmark(cpuName);

    public Task AddCpuBenchmark(CpuBenchmark cpuBenchmark) => _storageContext.AddCpuBenchmark(cpuBenchmark);

    public Task UpdateCpuBenchmark(CpuBenchmark cpuBenchmark) => _storageContext.UpdateCpuBenchmark(cpuBenchmark);

    public Task RemoveCpuBenchmark(Guid id) => _storageContext.RemoveCpuBenchmark(id);
}