using PCFromScratch.DBModels;

namespace PCFromScratch.Storage;

public interface IStorageContext
{
    IAsyncEnumerable<MotherboardRenamedForOmnissiah> GetMotherboards(string? socket = null);
    IAsyncEnumerable<Cpu> GetCpus(string? socket = null);
    IAsyncEnumerable<Gpu> GetGpus();
    IAsyncEnumerable<Ram> GetRams(string? generation = null);
    IAsyncEnumerable<InternalDrive> GetInternalDrives(string? type = null, int? capacity = null);
    IAsyncEnumerable<Cooler> GetCoolers(string? socket = null);
    IAsyncEnumerable<Psu> GetPsus(int? minPower = null);
    IAsyncEnumerable<CpuBenchmark> GetCpuBenchmarks(int? minScore = null);
    IAsyncEnumerable<GpuBenchmark> GetGpuBenchmarks(int? minScore = null);

    Task<MotherboardRenamedForOmnissiah?> GetMotherboard(Guid id);
    Task<Cpu?> GetCpu(Guid id);
    Task<Gpu?> GetGpu(Guid id);
    Task<Ram?> GetRam(Guid id);
    Task<InternalDrive?> GetInternalDrive(Guid id);
    Task<Cooler?> GetCooler(Guid id);
    Task<Psu?> GetPsu(Guid id);
    Task<CpuBenchmark?> GetCpuBenchmark(Guid id);
    Task<CpuBenchmark?> GetCpuBenchmark(string cpuName);
    Task<GpuBenchmark?> GetGpuBenchmark(Guid id);
    Task<GpuBenchmark?> GetGpuBenchmark(string gpuName);

    Task AddMotherboard(MotherboardRenamedForOmnissiah motherboardRenamedForOmnissiah);
    Task AddCpu(Cpu cpu);
    Task AddGpu(Gpu gpu);
    Task AddRam(Ram ram);
    Task AddInternalDrive(InternalDrive internalDrive);
    Task AddCooler(Cooler cooler);
    Task AddPsu(Psu psu);
    Task AddCpuBenchmark(CpuBenchmark cpuBenchmark);
    Task AddGpuBenchmark(GpuBenchmark gpuBenchmark);

    Task UpdateMotherboard(MotherboardRenamedForOmnissiah motherboardRenamedForOmnissiah);
    Task UpdateCpu(Cpu cpu);
    Task UpdateGpu(Gpu gpu);
    Task UpdateRam(Ram ram);
    Task UpdateInternalDrive(InternalDrive internalDrive);
    Task UpdateCooler(Cooler cooler);
    Task UpdatePsu(Psu psu);
    Task UpdateCpuBenchmark(CpuBenchmark cpuBenchmark);
    Task UpdateGpuBenchmark(GpuBenchmark gpuBenchmark);

    Task RemoveMotherboard(Guid id);
    Task RemoveCpu(Guid id);
    Task RemoveGpu(Guid id);
    Task RemoveRam(Guid id);
    Task RemoveInternalDrive(Guid id);
    Task RemoveCooler(Guid id);
    Task RemovePsu(Guid id);
    Task RemoveCpuBenchmark(Guid id);
    Task RemoveGpuBenchmark(Guid id);
}
