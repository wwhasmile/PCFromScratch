using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface ICpuBenchmarkService
{
    Task<CpuBenchmarkDtoModel?> GetCpuBenchmark(Guid id);
    Task<CpuBenchmarkDtoModel?> GetCpuBenchmark(string name);
}