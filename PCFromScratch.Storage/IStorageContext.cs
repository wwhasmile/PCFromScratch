using PCFromScratch.DBModels;

namespace PCFromScratch.Storage;

public interface IStorageContext
{
    IAsyncEnumerable<Motherboard> GetMotherboards();
    IAsyncEnumerable<Cpu> GetCpus();
    IAsyncEnumerable<Gpu> GetGpus();
    IAsyncEnumerable<Ram> GetRams();
    IAsyncEnumerable<Psu> GetPsus();

    Task<Motherboard?> GetMotherboard(Guid id);
    Task<Cpu?> GetCpu(Guid id);
    Task<Gpu?> GetGpu(Guid id);
    Task<Ram?> GetRam(Guid id);
    Task<Psu?> GetPsu(Guid id);

    Task AddMotherboard(Motherboard motherboard);
    Task AddCpu(Cpu cpu);
    Task AddGpu(Gpu gpu);
    Task AddRam(Ram ram);
    Task AddPsu(Psu psu);

    Task UpdateMotherboard(Motherboard motherboard);
    Task UpdateCpu(Cpu cpu);
    Task UpdateGpu(Gpu gpu);
    Task UpdateRam(Ram ram);
    Task UpdatePsu(Psu psu);

    Task RemoveMotherboard(Guid id);
    Task RemoveCpu(Guid id);
    Task RemoveGpu(Guid id);
    Task RemoveRam(Guid id);
    Task RemovePsu(Guid id);

    IAsyncEnumerable<Motherboard> GetMotherboardsBySocket(string socket);
    IAsyncEnumerable<Ram> GetRamByGeneration(string generation);
    IAsyncEnumerable<Psu> GetPsusFromPower(int minPower);
}
