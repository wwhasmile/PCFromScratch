using PCFromScratch.DBModels;
using Microsoft.EntityFrameworkCore;

namespace PCFromScratch.Storage;

public class EntityStorageContext(StorageDbContext dbContext) : IStorageContext
{
    private readonly StorageDbContext _dbContext = dbContext;

    public IAsyncEnumerable<Motherboard> GetMotherboards()
        => _dbContext.Motherboards.AsNoTracking().AsAsyncEnumerable();

    public IAsyncEnumerable<Cpu> GetCpus()
        => _dbContext.Cpus.AsNoTracking().AsAsyncEnumerable();

    public IAsyncEnumerable<Gpu> GetGpus()
        => _dbContext.Gpus.AsNoTracking().AsAsyncEnumerable();

    public IAsyncEnumerable<Ram> GetRams()
        => _dbContext.Rams.AsNoTracking().AsAsyncEnumerable();

    public IAsyncEnumerable<Psu> GetPsus()
        => _dbContext.Psus.AsNoTracking().AsAsyncEnumerable();

    public IAsyncEnumerable<Motherboard> GetMotherboardsBySocket(string socket)
        => _dbContext.Motherboards.AsNoTracking().Where(x => x.Socket == socket)
        .AsAsyncEnumerable();

    public IAsyncEnumerable<Ram> GetRamByGeneration(string generation)
        => _dbContext.Rams.AsNoTracking().Where(x => x.Generation == generation)
        .AsAsyncEnumerable();

    public IAsyncEnumerable<Psu> GetPsusFromPower(int minPower)
        => _dbContext.Psus.AsNoTracking().Where(x => x.Power >= minPower)
        .AsAsyncEnumerable();

    public async Task AddMotherboard(Motherboard motherboard)
    {
        _dbContext.Add(motherboard);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCpu(Cpu cpu)
    {
        _dbContext.Add(cpu);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddGpu(Gpu gpu)
    {
        _dbContext.Add(gpu);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddRam(Ram ram)
    {
        _dbContext.Add(ram);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddPsu(Psu psu)
    {
        _dbContext.Add(psu);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateMotherboard(Motherboard motherboard)
    {
        _dbContext.Motherboards.Update(motherboard);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCpu(Cpu cpu)
    {
        _dbContext.Cpus.Update(cpu);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateGpu(Gpu gpu)
    {
        _dbContext.Gpus.Update(gpu);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateRam(Ram ram)
    {
        _dbContext.Rams.Update(ram);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePsu(Psu psu)
    {
        _dbContext.Psus.Update(psu);
        await _dbContext.SaveChangesAsync();
    }
}