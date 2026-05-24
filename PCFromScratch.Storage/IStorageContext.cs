using PCFromScratch.DBModels;

namespace PCFromScratch.Storage;

public interface IStorageContext
{
    IAsyncEnumerable<Motherboard> GetMotherboards();
    IAsyncEnumerable<Cpu> GetCpus();
    IAsyncEnumerable<Gpu> GetGpus();
    IAsyncEnumerable<Ram> GetRams();
    IAsyncEnumerable<Psu> GetPsus();

    public Task AddMotherboard(Motherboard motherboard);
    public Task AddCpu(Cpu cpu);
    public Task AddGpu(Gpu gpu);
    public Task AddRam(Ram ram);
    public Task AddPsu(Psu psu);

    public Task UpdateMotherboard(Motherboard motherboard);
    public Task UpdateCpu(Cpu cpu);
    public Task UpdateGpu(Gpu gpu);
    public Task UpdateRam(Ram ram);
    public Task UpdatePsu(Psu psu);

    IAsyncEnumerable<Motherboard> GetMotherboardsBySocket(string socket);
    IAsyncEnumerable<Ram> GetRamByGeneration(string generation);
    IAsyncEnumerable<Psu> GetPsusFromPower(int minPower);
}
