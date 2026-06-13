using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface IGpuBenchmarkService
{
    IAsyncEnumerable<GpuBenchmarkDtoModel> GetGpuBenchmarks(int? minScore = null);
    Task<GpuBenchmarkDtoModel?> GetGpuBenchmark(Guid id);
    Task<GpuBenchmarkDtoModel?> GetGpuBenchmark(string name);
}