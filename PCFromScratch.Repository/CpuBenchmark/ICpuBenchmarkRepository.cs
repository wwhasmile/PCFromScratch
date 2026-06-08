using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface ICpuBenchmarkRepository
{
    IAsyncEnumerable<CpuBenchmark> GetCpuBenchmarks(int? minScore = null);

    Task<CpuBenchmark?> GetCpuBenchmark(Guid id);
    Task<CpuBenchmark?> GetCpuBenchmark(string cpuName);

    Task AddCpuBenchmark(CpuBenchmark cpuBenchmark);
    Task UpdateCpuBenchmark(CpuBenchmark cpuBenchmark);
    Task RemoveCpuBenchmark(Guid id);
}
