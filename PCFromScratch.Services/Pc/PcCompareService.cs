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
    IInternalDriveRepository internalDriveRepository,
    IPsuRepository psuRepository) : IPcCompareService
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

    public async Task<List<PcCompareMessage>> ComparePcs(PcDtoModel a, PcDtoModel b)
    {
        List<PcCompareMessage> result = [];

        await foreach (var message in CompareCpus(a.Cpu, b.Cpu))
            result.Add(message);

        await foreach (var message in CompareRams(a.Ram, b.Ram))
            result.Add(message);
        
        await foreach (var message in CompareGpus(a.Gpu, b.Gpu))
            result.Add(message);
        
        await foreach (var message in CompareDrives(a.InternalDrives, b.InternalDrives))
            result.Add(message);
        
        await foreach (var message in ComparePsus(a.Psu, b.Psu))
            result.Add(message);

        return result;
    }

    private async IAsyncEnumerable<PcCompareMessage> CompareCpus(Guid? a, Guid? b)
    {
        if (!a.HasValue || !b.HasValue) yield break;

        var cpuTaskA = cpuRepository.GetCpu(a.Value);
        var cpuTaskB = cpuRepository.GetCpu(b.Value);
        await Task.WhenAll(cpuTaskA, cpuTaskB);
        var cpuA = await cpuTaskA;
        var cpuB = await cpuTaskB;
        if (cpuA is null || cpuB is null) yield break;

        var cpuBenchmarkTaskA = cpuBenchmarkRepository.GetCpuBenchmark(cpuA.Name);
        var cpuBenchmarkTaskB = cpuBenchmarkRepository.GetCpuBenchmark(cpuB.Name);
        await Task.WhenAll(cpuBenchmarkTaskA, cpuBenchmarkTaskB);
        var cpuBenchmarkA = await cpuBenchmarkTaskA;
        var cpuBenchmarkB = await cpuBenchmarkTaskB;
        if (cpuBenchmarkA is null || cpuBenchmarkB is null) yield break;

        if (cpuBenchmarkA.Score < cpuBenchmarkB.Score)
            yield return new("Процесор", PcCompareMetric.Better);
        else if (cpuBenchmarkA.Score == cpuBenchmarkB.Score)
            yield return new("Процесор", PcCompareMetric.Equal);
        else
            yield return new("Процесор", PcCompareMetric.Worse);
    }

    private async IAsyncEnumerable<PcCompareMessage> CompareGpus(Guid? a, Guid? b)
    {
        if (!a.HasValue || !b.HasValue) yield break;

        var gpuTaskA = gpuRepository.GetGpu(a.Value);
        var gpuTaskB = gpuRepository.GetGpu(b.Value);
        await Task.WhenAll(gpuTaskA, gpuTaskB);
        var gpuA = await gpuTaskA;
        var gpuB = await gpuTaskB;
        if (gpuA is null || gpuB is null) yield break;

        var gpuBenchmarkTaskA = gpuBenchmarkRepository.GetGpuBenchmark(gpuA.Name);
        var gpuBenchmarkTaskB = gpuBenchmarkRepository.GetGpuBenchmark(gpuB.Name);
        await Task.WhenAll(gpuBenchmarkTaskA, gpuBenchmarkTaskB);
        var gpuBenchmarkA = await gpuBenchmarkTaskA;
        var gpuBenchmarkB = await gpuBenchmarkTaskB;
        if (gpuBenchmarkA is null || gpuBenchmarkB is null) yield break;

        if (gpuBenchmarkA.Score < gpuBenchmarkB.Score)
            yield return new("Відеокарта", PcCompareMetric.Better);
        else if (gpuBenchmarkA.Score == gpuBenchmarkB.Score)
            yield return new("Відеокарта", PcCompareMetric.Equal);
        else
            yield return new("Відеокарта", PcCompareMetric.Worse);
    }

    private async IAsyncEnumerable<PcCompareMessage> CompareRams(Guid? a, Guid? b)
    {
        if (!a.HasValue || !b.HasValue) yield break;

        var ramTaskA = ramRepository.GetRam(a.Value);
        var ramTaskB = ramRepository.GetRam(b.Value);
        await Task.WhenAll(ramTaskA, ramTaskB);
        var ramA = await ramTaskA;
        var ramB = await ramTaskB;
        if (ramA is null || ramB is null) yield break;

        if (ramA.Amount * ramA.Sticks > ramB.Amount * ramB.Sticks)
            yield return new("Оперативна пам'ять", PcCompareMetric.Better,
                "У вашій збірці більше оперативної пам'яті");
        else if (ramA.Amount * ramA.Sticks == ramB.Amount * ramB.Sticks)
            yield return new("Оперативна пам'ять", PcCompareMetric.Equal,
                "У вашій збірці така сама кількість оперативної пам'яті");
        else
            yield return new("Оперативна пам'ять", PcCompareMetric.Worse,
                "У вашій збірці менша кількість оперативної пам'яті");
        
        if (ramA.Frequency > ramB.Frequency)
            yield return new("Оперативна пам'ять", PcCompareMetric.Better,
                "У вашій збірці швидка оперативна пам'ять");
        else if (ramA.Frequency == ramB.Frequency)
            yield return new("Оперативна пам'ять", PcCompareMetric.Equal,
                "У вашій збірці така сама швидкість оперативної пам'яті");
        else
            yield return new("Оперативна пам'ять", PcCompareMetric.Worse,
                "У вашій збірці повільніша оперативна пам'ять");
        
        yield return ramA.Generation.CompareTo(ramB.Generation) switch
        {
            1 => new("Оперативна пам'ять", PcCompareMetric.Better,
                "У вашій збірці оперативна пам'ять більш нового покоління"),
            -1 => new("Оперативна пам'ять", PcCompareMetric.Worse,
                "У вашій збірці оперативна пам'ять більш старого покоління"),
            _ => new("Оперативна пам'ять", PcCompareMetric.Equal,
                "У вашій збірці оперативна пам'ять такого самого покоління"),
        };
    }

    private async IAsyncEnumerable<PcCompareMessage> CompareDrives(List<Guid> a, List<Guid> b)
    {
        var driveTasksA = a.Select(x => internalDriveRepository.GetInternalDrive(x));
        var driveTasksB = b.Select(x => internalDriveRepository.GetInternalDrive(x));

        var drivesA = await Task.WhenAll(driveTasksA);
        var drivesB = await Task.WhenAll(driveTasksB);

        var capacityA = drivesA.Sum(x => x?.Capacity ?? 0);
        var capacityB = drivesB.Sum(x => x?.Capacity ?? 0);

        if (capacityA > capacityB)
            yield return new("Диски", PcCompareMetric.Better,
                "У вашій збірці більше дискового простору");
        else if (capacityA == capacityB)
            yield return new("Диски", PcCompareMetric.Equal,
                "У вашій збірці такий самий дисковий простір");
        else
            yield return new("Диски", PcCompareMetric.Worse,
                "У вашій збірці менше дискового простору");
    }

    private async IAsyncEnumerable<PcCompareMessage> ComparePsus(Guid? a, Guid? b)
    {
        if (!a.HasValue || !b.HasValue) yield break;

        var psuTaskA = psuRepository.GetPsu(a.Value);
        var psuTaskB = psuRepository.GetPsu(b.Value);
        await Task.WhenAll(psuTaskA, psuTaskB);
        var psuA = await psuTaskA;
        var psuB = await psuTaskB;
        if (psuA is null || psuB is null) yield break;

        if (psuA.Power > psuB.Power)
            yield return new("Блок живлення", PcCompareMetric.Better,
                "У вашій збірці більш потужний блок живлення");
        else if (psuA.Power == psuB.Power)
            yield return new("Блок живлення", PcCompareMetric.Equal,
                "У вашій збірці так само потужний блок живлення");
        else
            yield return new("Блок живлення", PcCompareMetric.Worse,
                "У вашій збірці менш потужний блок живлення");
        
        if (psuA.Level > psuB.Level)
            yield return new("Блок живлення", PcCompareMetric.Better,
                "У вашій збірці краще сертифікований блок живлення");
        else if (psuA.Level == psuB.Level)
            yield return new("Блок живлення", PcCompareMetric.Equal,
                "У вашій збірці так само сертифікований блок живлення");
        else
            yield return new("Блок живлення", PcCompareMetric.Worse,
                "У вашій збірці гірше сертифікований блок живлення");
    }
}