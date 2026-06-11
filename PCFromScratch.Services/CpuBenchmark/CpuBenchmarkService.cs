using PCFromScratch.Common;
using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class CpuBenchmarkService(ICpuBenchmarkRepository cpuBenchmarkRepository) : ICpuBenchmarkService
{
    public async Task<CpuBenchmarkDtoModel?> GetCpuBenchmark(Guid id)
    {
        var cpuBenchmark = await cpuBenchmarkRepository.GetCpuBenchmark(id);
        if (cpuBenchmark is null) return null;

        return new(cpuBenchmark.Id, cpuBenchmark.Name, cpuBenchmark.Score);
    }

    public async Task<CpuBenchmarkDtoModel?> GetCpuBenchmark(string name)
    {
        var cpuBenchmark = await cpuBenchmarkRepository.GetCpuBenchmark(name);
        if (cpuBenchmark is null) return null;

        return new(cpuBenchmark.Id, cpuBenchmark.Name, cpuBenchmark.Score);
    }
}