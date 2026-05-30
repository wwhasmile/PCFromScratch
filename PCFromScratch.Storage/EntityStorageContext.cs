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

    public IAsyncEnumerable<InternalDrive> GetInternalDrives(string? type, int? capacity)
    {
        var query = _dbContext.InternalDrives.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(x => x.Type == type);
        }

        if (capacity.HasValue)
        {
            query = query.Where(x => x.Capacity == capacity.Value);
        }

        return query.AsAsyncEnumerable();
    }

    public async Task<Motherboard?> GetMotherboard(Guid id) => await _dbContext.FindAsync<Motherboard>(id);

    public async Task<Cpu?> GetCpu(Guid id) => await _dbContext.FindAsync<Cpu>(id);

    public async Task<Gpu?> GetGpu(Guid id) => await _dbContext.FindAsync<Gpu>(id);

    public async Task<Ram?> GetRam(Guid id) => await _dbContext.FindAsync<Ram>(id);

    public async Task<Psu?> GetPsu(Guid id) => await _dbContext.FindAsync<Psu>(id);

    public async Task<InternalDrive?> GetInternalDrive(Guid id) => await _dbContext.FindAsync<InternalDrive>(id);

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

    public async Task AddInternalDrive(InternalDrive internalDrive)
    {
        _dbContext.Add(internalDrive);
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

    public async Task UpdateInternalDrive(InternalDrive internalDrive)
    {
        _dbContext.InternalDrives.Update(internalDrive);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveMotherboard(Guid id)
        => await _dbContext.Motherboards.Where(x => x.Id == id).ExecuteDeleteAsync();

    public async Task RemoveCpu(Guid id)
        => await _dbContext.Cpus.Where(x => x.Id == id).ExecuteDeleteAsync();

    public async Task RemoveGpu(Guid id)
        => await _dbContext.Gpus.Where(x => x.Id == id).ExecuteDeleteAsync();

    public async Task RemoveRam(Guid id)
        => await _dbContext.Rams.Where(x => x.Id == id).ExecuteDeleteAsync();

    public async Task RemovePsu(Guid id)
        => await _dbContext.Psus.Where(x => x.Id == id).ExecuteDeleteAsync();

    public async Task RemoveInternalDrive(Guid id)
        => await _dbContext.InternalDrives.Where(x => x.Id == id).ExecuteDeleteAsync();
}