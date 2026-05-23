using PCFromScratch.DBModels;
using Microsoft.EntityFrameworkCore;

namespace PCFromScratch.Storage;

public class SqliteStorageContext(string connectionString) : IStorageContext
{
    private class SqliteDbContext(string connectionString) : DbContext
    {
        public DbSet<Cpu> Cpus { get; set; }
        public DbSet<Gpu> Gpus { get; set; }
        public DbSet<Ram> Rams { get; set; }
        public DbSet<Motherboard> Motherboards { get; set; }
        public DbSet<Psu> Psus { get; set; }

        private readonly string _connectionString = connectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={_connectionString}");
    }

    private readonly SqliteDbContext _dbContext = new(connectionString);

    public IAsyncEnumerable<Motherboard> GetMotherboards()
        => _dbContext.Motherboards.AsNoTracking<Motherboard>().AsAsyncEnumerable<Motherboard>();

    public IAsyncEnumerable<Cpu> GetCpus()
        => _dbContext.Cpus.AsNoTracking<Cpu>().AsAsyncEnumerable<Cpu>();

    public IAsyncEnumerable<Gpu> GetGpus()
        => _dbContext.Gpus.AsNoTracking<Gpu>().AsAsyncEnumerable<Gpu>();

    public IAsyncEnumerable<Ram> GetRams()
        => _dbContext.Rams.AsNoTracking<Ram>().AsAsyncEnumerable<Ram>();

    public IAsyncEnumerable<Psu> GetPsus()
        => _dbContext.Psus.AsNoTracking<Psu>().AsAsyncEnumerable<Psu>();

    public IAsyncEnumerable<Motherboard> GetMotherboardsBySocket(string socket)
        => _dbContext.Motherboards.AsNoTracking<Motherboard>().Where<Motherboard>(x => x.Socket == socket)
        .AsAsyncEnumerable<Motherboard>();

    public IAsyncEnumerable<Ram> GetRamByGeneration(string generation)
        => _dbContext.Rams.AsNoTracking<Ram>().Where<Ram>(x => x.Generation == generation)
        .AsAsyncEnumerable<Ram>();

    public IAsyncEnumerable<Psu> GetPsusFromPower(int minPower)
        => _dbContext.Psus.AsNoTracking<Psu>().Where<Psu>(x => x.Power >= minPower)
        .AsAsyncEnumerable<Psu>();
}