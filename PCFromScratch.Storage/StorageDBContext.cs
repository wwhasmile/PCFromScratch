using Microsoft.EntityFrameworkCore;

using PCFromScratch.DBModels;

namespace PCFromScratch.Storage;

public class StorageDbContext(DbContextOptions<StorageDbContext> options) : DbContext(options)
{
    public DbSet<Cpu> Cpus { get; set; }
    public DbSet<Gpu> Gpus { get; set; }
    public DbSet<Ram> Rams { get; set; }
    public DbSet<Motherboard> Motherboards { get; set; }
    public DbSet<Psu> Psus { get; set; }
}