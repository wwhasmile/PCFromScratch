using PCFromScratch.Common;
using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class PcCheckService(IMotherboardRepository motherboardRepository,
                            ICpuRepository cpuRepository,
                            IGpuRepository gpuRepository,
                            IRamRepository ramRepository,
                            ICoolerRepository coolerRepository,
                            IPsuRepository psuRepository) : IPcCheckService
{
    public async Task<List<Warning>> CheckPc(PcDtoModel pc)
    {
        List<Warning> warnings = [];

        var cpu = await (pc.Cpu.HasValue ? cpuRepository.GetCpu(pc.Cpu.Value) : Task.FromResult<Cpu?>(null));
        var motherboard = await (pc.Motherboard.HasValue ? motherboardRepository.GetMotherboard(pc.Motherboard.Value)
            : Task.FromResult<MotherboardRenamedForOmnissiah?>(null));
        var ram = await (pc.Ram.HasValue ? ramRepository.GetRam(pc.Ram.Value) : Task.FromResult<Ram?>(null));
        var gpu = await (pc.Gpu.HasValue ? gpuRepository.GetGpu(pc.Gpu.Value) : Task.FromResult<Gpu?>(null));
        var cooler = await (pc.Cooler.HasValue ? coolerRepository.GetCooler(pc.Cooler.Value)
                                        : Task.FromResult<Cooler?>(null));
        var psu = await (pc.Psu.HasValue ? psuRepository.GetPsu(pc.Psu.Value) : Task.FromResult<Psu?>(null));

        warnings.AddRange(ValidateMotherboard(cpu, motherboard));
        warnings.AddRange(ValidateRam(cpu, motherboard, ram));
        warnings.AddRange(ValidateCooler(cpu, motherboard, cooler));
        warnings.AddRange(ValidatePsu(psu, cpu, gpu));
        warnings.AddRange(GetCaseRequirements(motherboard, cooler, gpu));
        
        return warnings;
    }

    private static List<Warning> ValidateMotherboard(Cpu? cpu, MotherboardRenamedForOmnissiah? motherboard)
    {
        if (cpu is null) return [new(WarningSeverity.Info, "Процесор не обрано")];
        if (motherboard is null) return [new(WarningSeverity.Info, "Материнську плату не обрано")];

        if (cpu.Socket == motherboard.Socket) return [];

        return [ new(WarningSeverity.Incompatibility, $"Несумісний сокет процесора ({cpu.Socket}) та материнської плати ({motherboard.Socket})") ];
    }

    private static List<Warning> ValidateRam(Cpu? cpu, MotherboardRenamedForOmnissiah? motherboard, Ram? ram)
    {
        if (motherboard is null) return [];
        if (ram is null) return [new(WarningSeverity.Info, "Оперативну пам'ять не обрано")];

        List<Warning> result = [];
        if (motherboard.RamGeneration != ram.Generation)
            result.Add(new(WarningSeverity.Incompatibility,
                                        $"Несумісне покоління оперативної пам'яті ({ram.Generation}) з материнською платою ({motherboard.RamGeneration})"));
        if (motherboard.RamSlots < ram.Sticks)
            result.Add(new(WarningSeverity.Incompatibility,
                                        "Недостатня кількість слотів для оперативної пам'яті"));

        if (result.Count > 0) return result;

        int maxRamFrequency = motherboard.RamFrequency;
        if (cpu is not null && maxRamFrequency > cpu.RamFrequency)
            maxRamFrequency = cpu.RamFrequency;
        if (maxRamFrequency < ram.Frequency)
            result.Add(new(WarningSeverity.Bottleneck,
                            $"Частота оперативної пам'яті ({ram.Frequency}) більша за допустиму материнською платою ({maxRamFrequency})"));
        
        return result;
    }

    private static List<Warning> ValidateCooler(Cpu? cpu, MotherboardRenamedForOmnissiah? motherboard, Cooler? cooler)
    {
        if (motherboard is null) return [];
        if (cooler is null)
        {
            if (cpu is null || cpu.Packing == CpuPacking.Box) return [];
            return [new(WarningSeverity.Incompatibility, "До обраного процесора кулер не йде в комплекті")];
        }

        bool isCompatible = cooler.AmdSockets.Contains(motherboard.Socket) || 
                            cooler.IntelSockets.Contains(motherboard.Socket);
        if (!isCompatible)
            return [ new(WarningSeverity.Incompatibility, $"Кулер не підходить обраному сокету ({motherboard.Socket})") ];
        if (cpu?.Packing == CpuPacking.Box) return [new(WarningSeverity.Info, "Кулер йде в комплекті до обраного процесора")];
        List<Warning> warnings = [];
        if (cpu?.Tdp * 1.4 > cooler.Tdp)
            warnings.Add(new(WarningSeverity.Info,
                    "Обраний кулер може не забезпечити оптимальне охолодження (рекомендовано: 1.4x від споживання процесора)"
                    ));

        return warnings;
    }

    private static List<Warning> ValidatePsu(Psu? psu, Cpu? cpu, Gpu? gpu)
    {
        if (psu is null) return [new(WarningSeverity.Info, "Блок живлення не обрано")];
        if (gpu is null) return [new(WarningSeverity.Info, "Відеокарту не обрано")];

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

    private static List<Warning> GetCaseRequirements(MotherboardRenamedForOmnissiah? motherboard, Cooler? cooler, Gpu? gpu)
    {
        if (motherboard is null) return [];
        if (gpu is null) return [];
        string message = $"Вимоги до корпуса: {motherboard.FormFactor} форм фактор, {gpu.Length}мм довжина місця для відеокарти";
        if (cooler is not null && cooler.Height > 0) message += $", {cooler.Height} глибина";
        if (cooler is not null && cooler.Radius > 0) message += $", {cooler.Radius * cooler.FanCount}мм верхня панель";
        return [new(WarningSeverity.Info, message)];
    }
}