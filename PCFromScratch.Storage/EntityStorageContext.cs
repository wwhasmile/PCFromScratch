using FuzzySharp;

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

    public IAsyncEnumerable<Cpu> GetCpus(string? socket)
    {
        var query = _dbContext.Cpus.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(socket))
            query = query.Where(x => x.Socket == socket);

        return query.AsAsyncEnumerable();
    }

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

    public async Task<MotherboardRenamedForOmnissiah?> GetMotherboard(Guid id) => await _dbContext.Motherboards.Include(c => c.Offers).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Cpu?> GetCpu(Guid id)
        => await _dbContext.Cpus.Include(c => c.Offers).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Gpu?> GetGpu(Guid id)
        => await _dbContext.Gpus.Include(c => c.Offers).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Ram?> GetRam(Guid id)
                => await _dbContext.Rams.Include(c => c.Offers).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<InternalDrive?> GetInternalDrive(Guid id)
            => await _dbContext.InternalDrives.Include(c => c.Offers).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Cooler?> GetCooler(Guid id)
                => await _dbContext.Coolers.Include(c => c.Offers).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Psu?> GetPsu(Guid id)
                => await _dbContext.Psus.Include(c => c.Offers).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<CpuBenchmark?> GetCpuBenchmark(Guid id) => await _dbContext.FindAsync<CpuBenchmark>(id);

    public Task<CpuBenchmark?> GetCpuBenchmark(string cpuName)
    {
        var names = _dbContext.CpuBenchmarks.Select(n => n.Name).ToList();
        var result = Process.ExtractOne(cpuName, names);
        return _dbContext.CpuBenchmarks.FirstOrDefaultAsync(x => x.Name == result.Value);
    }

    public async Task<GpuBenchmark?> GetGpuBenchmark(Guid id) => await _dbContext.FindAsync<GpuBenchmark>(id);

    public Task<GpuBenchmark?> GetGpuBenchmark(string gpuName)
    {
        var names = _dbContext.GpuBenchmarks.Select(n => n.Name).ToList();
        var result = Process.ExtractOne(gpuName, names);
        return _dbContext.GpuBenchmarks.FirstOrDefaultAsync(x => x.Name == result.Value);
    }

    public async Task AddMotherboard(MotherboardRenamedForOmnissiah motherboardRenamedForOmnissiah)
    {
        _dbContext.Add(motherboardRenamedForOmnissiah);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCpu(Cpu cpu)
    {
        var existing = await _dbContext.Cpus.FirstOrDefaultAsync(x => x.Name == cpu.Name);

        if (existing is null)
            _dbContext.Add(cpu);
        else
        {
            cpu.Id = existing.Id;
            _dbContext.Entry(existing).CurrentValues.SetValues(cpu);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddGpu(Gpu gpu)
    {
        var existing = await _dbContext.Gpus.FirstOrDefaultAsync(x => x.Name == gpu.Name);

        if (existing is null)
            _dbContext.Add(gpu);
        else
        {
            gpu.Id = existing.Id;
            _dbContext.Entry(existing).CurrentValues.SetValues(gpu);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddRam(Ram ram)
    {
        var existing = await _dbContext.Rams.FirstOrDefaultAsync(x => x.Model == ram.Model);

        if (existing is null)
            _dbContext.Add(ram);
        else
        {
            ram.Id = existing.Id;
            _dbContext.Entry(existing).CurrentValues.SetValues(ram);
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddInternalDrive(InternalDrive internalDrive)
    {
        var existing = await _dbContext.InternalDrives.FirstOrDefaultAsync(x => x.Name == internalDrive.Name);

        if (existing is null)
            _dbContext.Add(internalDrive);
        else
        {
            internalDrive.Id = existing.Id;
            _dbContext.Entry(existing).CurrentValues.SetValues(internalDrive);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCooler(Cooler cooler)
    {
        var existing = await _dbContext.Coolers.FirstOrDefaultAsync(x => x.Name == cooler.Name);

        if (existing is null)
            _dbContext.Add(cooler);
        else
        {
            cooler.Id = existing.Id;
            _dbContext.Entry(existing).CurrentValues.SetValues(cooler);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddPsu(Psu psu)
    {
        var existing = await _dbContext.Psus.FirstOrDefaultAsync(x => x.Name == psu.Name);

        if (existing is null)
            _dbContext.Add(psu);
        else
        {
            psu.Id = existing.Id;
            _dbContext.Entry(existing).CurrentValues.SetValues(psu);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCpuBenchmark(CpuBenchmark cpuBenchmark)
    {
        var existing = await _dbContext.CpuBenchmarks.FirstOrDefaultAsync(x => x.Name == cpuBenchmark.Name);

        if (existing is null)
            _dbContext.Add(cpuBenchmark);
        else
        {
            cpuBenchmark.Id = existing.Id;
            _dbContext.Entry(existing).CurrentValues.SetValues(cpuBenchmark);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddGpuBenchmark(GpuBenchmark gpuBenchmark)
    {
        var existing = await _dbContext.GpuBenchmarks.FirstOrDefaultAsync(x => x.Name == gpuBenchmark.Name);

        if (existing is null)
            _dbContext.Add(gpuBenchmark);
        else
        {
            gpuBenchmark.Id = existing.Id;
            _dbContext.Entry(existing).CurrentValues.SetValues(gpuBenchmark);
        }
        
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