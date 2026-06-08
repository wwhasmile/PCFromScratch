using PCFromScratch.Common;
using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class PcCheckService(IMotherboardRepository motherboardRepository,
                            ICpuRepository cpuRepository,
                            IGpuRepository gpuRepository,
                            IRamRepository ramRepository,
                            IInternalDriveRepository internalDriveRepository,
                            ICoolerRepository coolerRepository,
                            IPsuRepository psuRepository) : IPcCheckService
{
    private readonly IMotherboardRepository _motherboardRepository = motherboardRepository;
    private readonly ICpuRepository _cpuRepository = cpuRepository;
    private readonly IGpuRepository _gpuRepository = gpuRepository;
    private readonly IRamRepository _ramRepository = ramRepository;
    private readonly IInternalDriveRepository _internalDriveRepository = internalDriveRepository;
    private readonly ICoolerRepository _coolerRepository = coolerRepository;
    private readonly IPsuRepository _psuRepository = psuRepository;

    public async Task<List<Warning>> CheckPc(PcDtoModel pc)
    {
        List<Warning> warnings = [];

        var cpuTask = pc.Cpu.HasValue ? _cpuRepository.GetCpu(pc.Cpu.Value) : Task.FromResult<Cpu?>(null);
        var motherboardTask = pc.Motherboard.HasValue ? _motherboardRepository.GetMotherboard(pc.Motherboard.Value)
            : Task.FromResult<MotherboardRenamedForOmnissiah?>(null);
        var ramTask = pc.Ram.HasValue ? _ramRepository.GetRam(pc.Ram.Value) : Task.FromResult<Ram?>(null);
        var gpuTask = pc.Gpu.HasValue ? _gpuRepository.GetGpu(pc.Gpu.Value) : Task.FromResult<Gpu?>(null);
        var coolerTask = pc.Cooler.HasValue ? _coolerRepository.GetCooler(pc.Cooler.Value)
            : Task.FromResult<Cooler?>(null);
        var psuTask = pc.Psu.HasValue ? _psuRepository.GetPsu(pc.Psu.Value) : Task.FromResult<Psu?>(null);

        await Task.WhenAll(cpuTask, motherboardTask, ramTask, coolerTask, psuTask);

        var cpu = await cpuTask;
        var motherboard = await motherboardTask;
        var ram = await ramTask;
        var gpu = await gpuTask;
        var cooler = await coolerTask;
        var psu = await psuTask;

        warnings.AddRange(ValidateMotherboard(cpu, motherboard));
        warnings.AddRange(ValidateRam(cpu, motherboard, ram));
        warnings.AddRange(ValidateCooler(cpu, motherboard, cooler));
        warnings.AddRange(ValidatePsu(psu, cpu, gpu));

        return warnings;
    }

    private static List<Warning> ValidateMotherboard(Cpu? cpu, MotherboardRenamedForOmnissiah? motherboard)
    {
        if (cpu is null) return [];
        if (motherboard is null) return [];

        if (cpu.Socket == motherboard.Socket) return [];

        return [ new(WarningSeverity.Incompatibility, "Несумісний сокет процесора та материнської плати") ];
    }

    private static List<Warning> ValidateRam(Cpu? cpu, MotherboardRenamedForOmnissiah? motherboard, Ram? ram)
    {
        if (motherboard is null) return [];
        if (ram is null) return [];

        List<Warning> result = [];
        if (motherboard.RamGeneration != ram.Generation)
            result.Add(new(WarningSeverity.Incompatibility,
                                        "Несумісне покоління оперативної пам'яті з материнською платою"));
        if (motherboard.RamSlots < ram.Sticks)
            result.Add(new(WarningSeverity.Incompatibility,
                                        "Недостатня кількість слотів для оперативної пам'яті"));

        if (result.Count > 0) return result;

        int maxRamFrequency = motherboard.RamFrequency;
        if (cpu is not null && maxRamFrequency > cpu.RamFrequency)
            maxRamFrequency = cpu.RamFrequency;
        if (maxRamFrequency / ram.Frequency > 1.35)
            result.Add(new(WarningSeverity.Bottleneck,
                                        "Частота оперативної пам'яті нижча за рекомендовану"));
        else if (maxRamFrequency < ram.Frequency)
            result.Add(new(WarningSeverity.Bottleneck,
                            "Частота оперативної пам'яті переважає максимально допустиму"));
        
        return result;
    }

    private static List<Warning> ValidateCooler(Cpu? cpu, MotherboardRenamedForOmnissiah? motherboard, Cooler? cooler)
    {
        if (motherboard is null) return [];
        if (cooler is null) return [];

        bool isCompatible = cooler.AmdSockets.Contains(motherboard.Socket) || 
                            cooler.IntelSockets.Contains(motherboard.Socket);
        if (!isCompatible)
            return [ new(WarningSeverity.Incompatibility, "Несумісний сокет кулера та материнської плати") ];

        List<Warning> warnings = [];
        if (cpu?.Tdp * 1.4 > cooler.Tdp)
            warnings.Add(new(WarningSeverity.Info,
                    "Обраний кулер може не забезпечити оптимальний тепловий запас (рекомендовано: 1.4x TDP процесора)"
                    ));

        return warnings;
    }

    private static List<Warning> ValidatePsu(Psu? psu, Cpu? cpu, Gpu? gpu)
    {
        if (psu is null) return [];

        var totalPower = 150;
        if (cpu is not null)
        totalPower += cpu.Tdp;
        if (gpu is not null)
        totalPower += gpu.Tdp * 2;
        totalPower += (int)(totalPower * 0.4);

        if (totalPower >= psu.Power) return [];

        return [ new(WarningSeverity.Incompatibility,
                    "Обраний блок живлення не є достатньо потужним для інших обраних компонентів."
                    ) ];
    }
}