using PCFromScratch.DBModels;
using Microsoft.EntityFrameworkCore;

namespace PCFromScratch.Storage;

public class EntityStorageContext(StorageDbContext dbContext) : IStorageContext
{
    private readonly StorageDbContext _dbContext = dbContext;

    public IAsyncEnumerable<MotherboardRenamedForOmnissiah> GetMotherboards(string? socket)
    {
        var query = _dbContext.Motherboards.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(socket))
            query = query.Where(x => x.Socket == socket);

        return query.AsAsyncEnumerable();
    }

    public IAsyncEnumerable<Cpu> GetCpus()
        => _dbContext.Cpus.AsNoTracking().AsAsyncEnumerable();

    public IAsyncEnumerable<Gpu> GetGpus()
        => _dbContext.Gpus.AsNoTracking().AsAsyncEnumerable();

    public IAsyncEnumerable<Ram> GetRams(string? generation)
    {
        var query = _dbContext.Rams.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(generation))
            query = query.Where(x => x.Generation == generation);

        return query.AsAsyncEnumerable();
    }

    public IAsyncEnumerable<InternalDrive> GetInternalDrives(string? type, int? capacity)
    {
        var query = _dbContext.InternalDrives.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(x => x.Type == type);

        if (capacity.HasValue)
            query = query.Where(x => x.Capacity == capacity.Value);

        return query.AsAsyncEnumerable();
    }

    public IAsyncEnumerable<Cooler> GetCoolers(string? socket = null)
    {
        var query = _dbContext.Coolers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(socket))
            query = query.Where(x => x.AmdSockets.Contains(socket) || x.IntelSockets.Contains(socket));

        return query.AsAsyncEnumerable();
    }

    public IAsyncEnumerable<Psu> GetPsus(int? minPower)
    {
        var query = _dbContext.Psus.AsNoTracking();

        if (minPower.HasValue)
            query = query.Where(x => x.Power >= minPower);

        return query.AsAsyncEnumerable();
    }

    public IAsyncEnumerable<CpuBenchmark> GetCpuBenchmarks(int? minScore)
    {
        var query = _dbContext.CpuBenchmarks.AsNoTracking();

        if (minScore.HasValue)
            query = query.Where(x => x.Score >= minScore);
        
        return query.AsAsyncEnumerable();
    }

    public IAsyncEnumerable<GpuBenchmark> GetGpuBenchmarks(int? minScore)
    {
        var query = _dbContext.GpuBenchmarks.AsNoTracking();

        if (minScore.HasValue)
            query = query.Where(x => x.Score >= minScore);
        
        return query.AsAsyncEnumerable();
    }

    public async Task<MotherboardRenamedForOmnissiah?> GetMotherboard(Guid id) => await _dbContext.FindAsync<MotherboardRenamedForOmnissiah>(id);

    public async Task<Cpu?> GetCpu(Guid id) => await _dbContext.FindAsync<Cpu>(id);

    public async Task<Gpu?> GetGpu(Guid id) => await _dbContext.FindAsync<Gpu>(id);

    public async Task<Ram?> GetRam(Guid id) => await _dbContext.FindAsync<Ram>(id);

    public async Task<InternalDrive?> GetInternalDrive(Guid id) => await _dbContext.FindAsync<InternalDrive>(id);

    public async Task<Cooler?> GetCooler(Guid id) => await _dbContext.FindAsync<Cooler>(id);

    public async Task<Psu?> GetPsu(Guid id) => await _dbContext.FindAsync<Psu>(id);

    public async Task<CpuBenchmark?> GetCpuBenchmark(Guid id) => await _dbContext.FindAsync<CpuBenchmark>(id);

    public async Task<GpuBenchmark?> GetGpuBenchmark(Guid id) => await _dbContext.FindAsync<GpuBenchmark>(id);

    public async Task AddMotherboard(MotherboardRenamedForOmnissiah motherboardRenamedForOmnissiah)
    {
        _dbContext.Add(motherboardRenamedForOmnissiah);
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

    public async Task AddInternalDrive(InternalDrive internalDrive)
    {
        _dbContext.Add(internalDrive);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCooler(Cooler cooler)
    {
        _dbContext.Add(cooler);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddPsu(Psu psu)
    {
        _dbContext.Add(psu);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCpuBenchmark(CpuBenchmark cpuBenchmark)
    {
        _dbContext.Add(cpuBenchmark);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddGpuBenchmark(GpuBenchmark gpuBenchmark)
    {
        _dbContext.Add(gpuBenchmark);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateMotherboard(MotherboardRenamedForOmnissiah motherboardRenamedForOmnissiah)
    {
        _dbContext.Motherboards.Update(motherboardRenamedForOmnissiah);
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

    public async Task UpdateInternalDrive(InternalDrive internalDrive)
    {
        _dbContext.InternalDrives.Update(internalDrive);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCooler(Cooler cooler)
    {
        _dbContext.Coolers.Update(cooler);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePsu(Psu psu)
    {
        _dbContext.Psus.Update(psu);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCpuBenchmark(CpuBenchmark cpuBenchmark)
    {
        _dbContext.CpuBenchmarks.Update(cpuBenchmark);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateGpuBenchmark(GpuBenchmark gpuBenchmark)
    {
        _dbContext.GpuBenchmarks.Update(gpuBenchmark);
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

    public async Task RemoveInternalDrive(Guid id)
        => await _dbContext.InternalDrives.Where(x => x.Id == id).ExecuteDeleteAsync();

    public async Task RemoveCooler(Guid id)
        => await _dbContext.Coolers.Where(x => x.Id == id).ExecuteDeleteAsync();

    public async Task RemovePsu(Guid id)
        => await _dbContext.Psus.Where(x => x.Id == id).ExecuteDeleteAsync();
    
    public async Task RemoveCpuBenchmark(Guid id)
        => await _dbContext.CpuBenchmarks.Where(x => x.Id == id).ExecuteDeleteAsync();

    public async Task RemoveGpuBenchmark(Guid id)
        => await _dbContext.GpuBenchmarks.Where(x => x.Id == id).ExecuteDeleteAsync();
}