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
    public async Task<RequirementsResultDtoModel> IsFitRequirements(PcDtoModel pc, SystemRequirementsDtoModel requirements)
    {
        var result = true;
        var messages = new ConcurrentDictionary<string, string>();

        var checkCpu = await CheckCpu(pc.Cpu, requirements.CpuBenchmark, messages);
        var checkGpu = await CheckGpu(pc.Gpu, requirements.GpuBenchmark, messages);
        var checkRam = await CheckRam(pc.Ram, requirements.RamInMegabytes, messages);
        var checkDrives = await CheckDrives(pc.InternalDrives, requirements.SpaceOnDiskInGigabytes,
            requirements.SsdRequired, messages);
        
        if (!checkCpu || !checkGpu || !checkRam || !checkDrives)
            result = false;

        return new RequirementsResultDtoModel(result, messages.ToDictionary());
    }

    private async Task<bool> CheckCpu(Guid? cpuId, Guid? cpuBenchmarkId, ConcurrentDictionary<string, string> messages)
    {
        if (!cpuId.HasValue) return true;
        if (!cpuBenchmarkId.HasValue) return true;
        
        var cpu = await cpuRepository.GetCpu(cpuId.Value);
        var targetCpuBenchmark = await cpuBenchmarkRepository.GetCpuBenchmark(cpuBenchmarkId.Value);

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

        var gpu = await gpuRepository.GetGpu(gpuId.Value);
        var targetGpuBenchmark = await gpuBenchmarkRepository.GetGpuBenchmark(gpuBenchmarkId.Value);

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
        List<InternalDrive> drives = [];
        foreach (var id in driveIds)
        {
            var drive = await internalDriveRepository.GetInternalDrive(id);
            if (drive is not null) drives.Add(drive);
        }
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

        foreach (var message in await CompareCpus(a.Cpu, b.Cpu).ToListAsync())
            result.Add(message);

        foreach (var message in await CompareRams(a.Ram, b.Ram).ToListAsync())
            result.Add(message);
        
        foreach (var message in await CompareGpus(a.Gpu, b.Gpu).ToListAsync())
            result.Add(message);
        
        result.Add(await CompareDrives(a.InternalDrives, b.InternalDrives));
        
        foreach (var message in await ComparePsus(a.Psu, b.Psu).ToListAsync())
            result.Add(message);

        return result;
    }

    private async IAsyncEnumerable<PcCompareMessage> CompareCpus(Guid? a, Guid? b)
    {
        if (!a.HasValue || !b.HasValue) yield break;
        
        var cpuA = await cpuRepository.GetCpu(a.Value);
        var cpuB = await cpuRepository.GetCpu(b.Value);
        if (cpuA is null || cpuB is null) yield break;
        
        var cpuBenchmarkA = await cpuBenchmarkRepository.GetCpuBenchmark(cpuA.Name);
        var cpuBenchmarkB = await cpuBenchmarkRepository.GetCpuBenchmark(cpuB.Name);
        if (cpuBenchmarkA is null || cpuBenchmarkB is null) yield break;

        if (cpuBenchmarkA.Score < cpuBenchmarkB.Score)
            yield return new("Cpu", PcCompareMetric.Better);
        else if (cpuBenchmarkA.Score == cpuBenchmarkB.Score)
            yield return new("Cpu", PcCompareMetric.Equal);
        else
            yield return new("Cpu", PcCompareMetric.Worse);
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
            yield return new("Gpu", PcCompareMetric.Better);
        else if (gpuBenchmarkA.Score == gpuBenchmarkB.Score)
            yield return new("Gpu", PcCompareMetric.Equal);
        else
            yield return new("Gpu", PcCompareMetric.Worse);
    }

    private async IAsyncEnumerable<PcCompareMessage> CompareRams(Guid? a, Guid? b)
    {
        if (!a.HasValue || !b.HasValue) yield break;

        var ramA = await ramRepository.GetRam(a.Value);
        var ramB = await ramRepository.GetRam(b.Value);
        if (ramA is null || ramB is null) yield break;

        if (ramA.Amount * ramA.Sticks > ramB.Amount * ramB.Sticks)
            yield return new("Ram", PcCompareMetric.Better,
                "У вашій збірці більше оперативної пам'яті");
        else if (ramA.Amount * ramA.Sticks == ramB.Amount * ramB.Sticks)
            yield return new("Ram", PcCompareMetric.Equal,
                "У вашій збірці така сама кількість оперативної пам'яті");
        else
            yield return new("Ram", PcCompareMetric.Worse,
                "У вашій збірці менша кількість оперативної пам'яті");
        
        if (ramA.Frequency > ramB.Frequency)
            yield return new("Ram", PcCompareMetric.Better,
                "У вашій збірці швидка оперативна пам'ять");
        else if (ramA.Frequency == ramB.Frequency)
            yield return new("Ram", PcCompareMetric.Equal,
                "У вашій збірці така сама швидкість оперативної пам'яті");
        else
            yield return new("Ram", PcCompareMetric.Worse,
                "У вашій збірці повільніша оперативна пам'ять");
        
        yield return ramA.Generation.CompareTo(ramB.Generation) switch
        {
            1 => new("Ram", PcCompareMetric.Better,
                "У вашій збірці оперативна пам'ять більш нового покоління"),
            -1 => new("Ram", PcCompareMetric.Worse,
                "У вашій збірці оперативна пам'ять більш старого покоління"),
            _ => new("Ram", PcCompareMetric.Equal,
                "У вашій збірці оперативна пам'ять такого самого покоління"),
        };
    }

    private async Task<PcCompareMessage> CompareDrives(List<Guid> a, List<Guid> b)
    {
        List<InternalDrive> drivesA = [];
        foreach (var id in a)
        {
            var drive = await internalDriveRepository.GetInternalDrive(id);
            if (drive is not null) drivesA.Add(drive);
        }
        List<InternalDrive> drivesB = [];
        foreach (var id in b)
        {
            var drive = await internalDriveRepository.GetInternalDrive(id);
            if (drive is not null) drivesB.Add(drive);
        }

        var capacityA = drivesA.Sum(x => x.Capacity);
        var capacityB = drivesB.Sum(x => x.Capacity);

        if (capacityA > capacityB)
            return new PcCompareMessage("Storage", PcCompareMetric.Better,
                "У вашій збірці більше дискового простору");
        if (capacityA == capacityB)
            return new PcCompareMessage("Storage", PcCompareMetric.Equal,
                "У вашій збірці такий самий дисковий простір");
        return new PcCompareMessage("Storage", PcCompareMetric.Worse,
                "У вашій збірці менше дискового простору");
    }

    private async IAsyncEnumerable<PcCompareMessage> ComparePsus(Guid? a, Guid? b)
    {
        if (!a.HasValue || !b.HasValue) yield break;
        
        var psuA = await psuRepository.GetPsu(a.Value);
        var psuB = await psuRepository.GetPsu(b.Value);
        if (psuA is null || psuB is null) yield break;

        if (psuA.Power > psuB.Power)
            yield return new("Psu", PcCompareMetric.Better,
                "У вашій збірці більш потужний блок живлення");
        else if (psuA.Power == psuB.Power)
            yield return new("Psu", PcCompareMetric.Equal,
                "У вашій збірці так само потужний блок живлення");
        else
            yield return new("Psu", PcCompareMetric.Worse,
                "У вашій збірці менш потужний блок живлення");
        
        if (psuA.Level > psuB.Level)
            yield return new("Psu", PcCompareMetric.Better,
                "У вашій збірці краще сертифікований блок живлення");
        else if (psuA.Level == psuB.Level)
            yield return new("Psu", PcCompareMetric.Equal,
                "У вашій збірці так само сертифікований блок живлення");
        else
            yield return new("Psu", PcCompareMetric.Worse,
                "У вашій збірці гірше сертифікований блок живлення");
    }
}