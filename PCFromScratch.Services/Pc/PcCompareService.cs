using System.Collections.Concurrent;

using PCFromScratch.Common;
using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class PcCompareService(ICpuRepository cpuRepository,
    IGpuRepository gpuRepository,
    ICpuBenchmarkRepository cpuBenchmarkRepository,
    IGpuBenchmarkRepository gpuBenchmarkRepository,
    IRamRepository ramRepository,
    IInternalDriveRepository internalDriveRepository) : IPcCompareService
{
    public async Task<(bool, Dictionary<string, string>)> IsFitRequirements(PcDtoModel pc, SystemRequirementsDtoModel requirements)
    {
        var result = true;
        var messages = new ConcurrentDictionary<string, string>();

        var checkCpuTask = CheckCpu(pc.Cpu, requirements.CpuBenchmark, messages);
        var checkGpuTask = CheckGpu(pc.Gpu, requirements.GpuBenchmark, messages);
        var checkRamTask = CheckRam(pc.Ram, requirements.RamInMegabytes, messages);
        var checkDrivesTask = CheckDrives(pc.InternalDrives, requirements.SpaceOnDiskInGigabytes,
            requirements.SsdRequired, messages);

        await Task.WhenAll(checkCpuTask, checkGpuTask, checkRamTask, checkDrivesTask);
        if (!await checkCpuTask || !await checkGpuTask || !await checkRamTask || !await checkDrivesTask)
            result = false;

        return (result, messages.ToDictionary());
    }

    private async Task<bool> CheckCpu(Guid? cpuId, Guid? cpuBenchmarkId, ConcurrentDictionary<string, string> messages)
    {
        if (!cpuId.HasValue) return true;
        if (!cpuBenchmarkId.HasValue) return true;
        
        var cpuTask = cpuRepository.GetCpu(cpuId.Value);
        var targetCpuBenchmarkTask = cpuBenchmarkRepository.GetCpuBenchmark(cpuBenchmarkId.Value);
        await Task.WhenAll(cpuTask, targetCpuBenchmarkTask);

        var cpu = await cpuTask;
        var targetCpuBenchmark = await targetCpuBenchmarkTask;

        if (cpu is null) return true;
        if (targetCpuBenchmark is null) return true;

        var cpuBenchmark = await cpuBenchmarkRepository.GetCpuBenchmark(cpu.Name);
        if (cpuBenchmark is null) return true;

        if (cpuBenchmark.Score >= targetCpuBenchmark.Score) return true;

        messages.TryAdd("CPU", "Процесор не відповідає вимогам потужності");
        return false;
    }

    private async Task<bool> CheckGpu(Guid? gpuId, Guid? gpuBenchmarkId, ConcurrentDictionary<string, string> messages)
    {
        if (!gpuId.HasValue) return true;
        if (!gpuBenchmarkId.HasValue) return true;
        
        var gpuTask = gpuRepository.GetGpu(gpuId.Value);
        var targetGpuBenchmarkTask = gpuBenchmarkRepository.GetGpuBenchmark(gpuBenchmarkId.Value);
        await Task.WhenAll(gpuTask, targetGpuBenchmarkTask);

        var gpu = await gpuTask;
        var targetGpuBenchmark = await targetGpuBenchmarkTask;

        if (gpu is null) return true;
        if (targetGpuBenchmark is null) return true;

        var gpuBenchmark = await gpuBenchmarkRepository.GetGpuBenchmark(gpu.Name);
        if (gpuBenchmark is null) return true;

        if (gpuBenchmark.Score >= targetGpuBenchmark.Score) return true;

        messages.TryAdd("GPU", "Відеокарта не відповідає вимогам потужності");
        return false;
    }

    private async Task<bool> CheckRam(Guid? ramId, int ramInMb, ConcurrentDictionary<string, string> messages)
    {
        if (!ramId.HasValue) return true;
        
        var ram = await ramRepository.GetRam(ramId.Value);
        if (ram is null) return true;

        if (ram.Amount * ram.Sticks >= ramInMb) return true;

        messages.TryAdd("RAM", "Кількість оперативної пам'яті не відповідає вимогам");
        return false;
    }

    private async Task<bool> CheckDrives(IEnumerable<Guid> driveIds, int capacity, bool requireSsd,
            ConcurrentDictionary<string, string> messages)
    {
        var drives = await Task.WhenAll(driveIds.Select(id => internalDriveRepository.GetInternalDrive(id)));
        var totalSize = 0;

        var hasSsd = false;
        foreach (var drive in drives)
        {
            totalSize += drive?.Capacity ?? 0;
            if (drive?.Type == "SSD") hasSsd = true;
        }
        
        var result = true;
        if (!hasSsd && requireSsd)
        {
            messages.TryAdd("SSD", "Відсутній SSD");
            result = false;
        }

        if (totalSize >= capacity) return result;

        messages.TryAdd("Диски", "Загальна кількість місця на накопичувачах не відповідає вимогам");
        return false;
    }

    public async IAsyncEnumerable<PcCompareMessage> ComparePcs(PcDtoModel a, PcDtoModel b)
    {
        var tasks = await Task.WhenAll(CompareCpus(a.Cpu, b.Cpu), CompareGpus(a.Gpu, b.Gpu));

        foreach (var message in tasks)
            if (message.HasValue)
                yield return message.Value;
    }

    private async Task<PcCompareMessage?> CompareCpus(Guid? a, Guid? b)
    {
        if (!a.HasValue || !b.HasValue) return null;

        var cpuTaskA = cpuRepository.GetCpu(a.Value);
        var cpuTaskB = cpuRepository.GetCpu(b.Value);
        await Task.WhenAll(cpuTaskA, cpuTaskB);
        var cpuA = await cpuTaskA;
        var cpuB = await cpuTaskB;
        if (cpuA is null || cpuB is null) return null;

        var cpuBenchmarkTaskA = cpuBenchmarkRepository.GetCpuBenchmark(cpuA.Name);
        var cpuBenchmarkTaskB = cpuBenchmarkRepository.GetCpuBenchmark(cpuB.Name);
        await Task.WhenAll(cpuBenchmarkTaskA, cpuBenchmarkTaskB);
        var cpuBenchmarkA = await cpuBenchmarkTaskA;
        var cpuBenchmarkB = await cpuBenchmarkTaskB;
        if (cpuBenchmarkA is null || cpuBenchmarkB is null) return null;

        if (cpuBenchmarkA.Score < cpuBenchmarkB.Score)
            return new("Процесор", PcCompareMetric.Better);
        else if (cpuBenchmarkA.Score == cpuBenchmarkB.Score)
            return new("Процесор", PcCompareMetric.Equal);
        else
            return new("Процесор", PcCompareMetric.Worse);
    }

    private async Task<PcCompareMessage?> CompareGpus(Guid? a, Guid? b)
    {
        if (!a.HasValue || !b.HasValue) return null;

        var gpuTaskA = gpuRepository.GetGpu(a.Value);
        var gpuTaskB = gpuRepository.GetGpu(b.Value);
        await Task.WhenAll(gpuTaskA, gpuTaskB);
        var gpuA = await gpuTaskA;
        var gpuB = await gpuTaskB;
        if (gpuA is null || gpuB is null) return null;

        var gpuBenchmarkTaskA = gpuBenchmarkRepository.GetGpuBenchmark(gpuA.Name);
        var gpuBenchmarkTaskB = gpuBenchmarkRepository.GetGpuBenchmark(gpuB.Name);
        await Task.WhenAll(gpuBenchmarkTaskA, gpuBenchmarkTaskB);
        var gpuBenchmarkA = await gpuBenchmarkTaskA;
        var gpuBenchmarkB = await gpuBenchmarkTaskB;
        if (gpuBenchmarkA is null || gpuBenchmarkB is null) return null;

        if (gpuBenchmarkA.Score < gpuBenchmarkB.Score)
            return new("Відеокарта", PcCompareMetric.Better);
        else if (gpuBenchmarkA.Score == gpuBenchmarkB.Score)
            return new("Відеокарта", PcCompareMetric.Equal);
        else
            return new("Відеокарта", PcCompareMetric.Worse);
    }
}