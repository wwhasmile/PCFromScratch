using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface ICpuBenchmarkService
{
    IAsyncEnumerable<CpuBenchmarkDtoModel> GetCpuBenchmarks(int? minScore = null);
    Task<CpuBenchmarkDtoModel?> GetCpuBenchmark(Guid id);
    Task<CpuBenchmarkDtoModel?> GetCpuBenchmark(string name);
}