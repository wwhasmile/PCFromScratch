using PCFromScratch.DBModels;

namespace PCFromScratch.Storage;

public interface IStorageContext
{
    IAsyncEnumerable<Motherboard> GetMotherboards();
    IAsyncEnumerable<Cpu> GetCpus();
    IAsyncEnumerable<Gpu> GetGpus();
    IAsyncEnumerable<Ram> GetRams();
    IAsyncEnumerable<Psu> GetPsus();

    IAsyncEnumerable<Motherboard> GetMotherboardsBySocket(string socket);
    IAsyncEnumerable<Ram> GetRamByGeneration(string generation);
    IAsyncEnumerable<Psu> GetPsusFromPower(int minPower);
}
