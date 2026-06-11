using PCFromScratch.Common;
using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class GpuBenchmarkService(IGpuBenchmarkRepository gpuBenchmarkRepository) : IGpuBenchmarkService
{
    public async Task<GpuBenchmarkDtoModel?> GetGpuBenchmark(Guid id)
    {
        var gpuBenchmark = await gpuBenchmarkRepository.GetGpuBenchmark(id);
        if (gpuBenchmark is null) return null;

        return new(gpuBenchmark.Id, gpuBenchmark.Name, gpuBenchmark.Score);
    }

    public async Task<GpuBenchmarkDtoModel?> GetGpuBenchmark(string name)
    {
        var gpuBenchmark = await gpuBenchmarkRepository.GetGpuBenchmark(name);
        if (gpuBenchmark is null) return null;

        return new(gpuBenchmark.Id, gpuBenchmark.Name, gpuBenchmark.Score);
    }
}