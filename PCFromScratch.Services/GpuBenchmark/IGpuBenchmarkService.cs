using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface IGpuBenchmarkService
{
    Task<GpuBenchmarkDtoModel?> GetGpuBenchmark(Guid id);
    Task<GpuBenchmarkDtoModel?> GetGpuBenchmark(string name);
}