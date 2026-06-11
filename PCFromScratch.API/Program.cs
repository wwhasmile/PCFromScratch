using Microsoft.EntityFrameworkCore;

using PCFromScratch.API.Services;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;
using PCFromScratch.Services;
using PCFromScratch.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<StorageDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("PCFromScratch.Migrations")));
builder.Services.AddScoped<IStorageContext, EntityStorageContext>();
builder.Services.AddScoped<ICpuRepository, StorageCpuRepository>();
builder.Services.AddScoped<ICpuBenchmarkRepository, CpuBenchmarkRepository>();
builder.Services.AddScoped<IRamRepository, StorageRamRepository>();
builder.Services.AddScoped<IMotherboardRepository, StorageMotherboardRepository>();
builder.Services.AddScoped<ICoolerRepository, StorageCoolerRepository>();
builder.Services.AddScoped<IGpuRepository, StorageGpuRepository>();
builder.Services.AddScoped<IGpuBenchmarkRepository, GpuBenchmarkRepository>();
builder.Services.AddScoped<IInternalDriveRepository, StorageInternalDriveRepository>();
builder.Services.AddScoped<IPsuRepository, StoragePsuRepository>();

builder.Services.AddScoped<ICpuService, CpuService>();
builder.Services.AddScoped<ICpuBenchmarkService, CpuBenchmarkService>();
builder.Services.AddScoped<IRamService, RamService>();
builder.Services.AddScoped<IMotherboardService, MotherboardService>();
builder.Services.AddScoped<ICoolerService, CoolerService>();
builder.Services.AddScoped<IGpuService, GpuService>();
builder.Services.AddScoped<IGpuBenchmarkService, GpuBenchmarkService>();
builder.Services.AddScoped<IInternalDriveService, InternalDriveService>();
builder.Services.AddScoped<IPsuService, PsuService>();

builder.Services.AddHostedService<CpuScraperBackgroundService>();
builder.Services.AddHostedService<RamScraperBackgroundService>();
builder.Services.AddHostedService<MotherboardScraperBackgroundService>();
builder.Services.AddHostedService<CoolerScraperBackgroundService>();
builder.Services.AddHostedService<GpuScraperBackgroundService>();
builder.Services.AddHostedService<HddScraperBackgroundService>();
builder.Services.AddHostedService<SsdScraperBackgroundService>();
builder.Services.AddHostedService<PsuScraperBackgroundService>();

builder.Services.AddScoped<IPcCheckService, PcCheckService>();
builder.Services.AddScoped<IPcCompareService, PcCompareService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/cpu", async (string? socket, ICpuService cpuService) => await cpuService.GetCpus(socket).ToListAsync());
app.MapGet("/cpu/{id}", async (Guid id, ICpuService cpuService) =>
{
    var cpu = await cpuService.GetCpu(id);
    return cpu is null ? Results.NotFound() : Results.Ok(cpu);
});
app.MapGet("/cpu/{id}/offers", async (Guid id, ICpuService cpuService) =>
    await cpuService.GetCpuOffers(id).ToListAsync());

app.MapGet("/ram", async (string? generation, IRamService ramService) =>
    await ramService.GetRams(generation).ToListAsync());
app.MapGet("/ram/{id}", async (Guid id, IRamService ramService) =>
{
    var ram = await ramService.GetRam(id);
    return ram is null ? Results.NotFound() : Results.Ok(ram);
});
app.MapGet("/ram/{id}/offers", async (Guid id, IRamService ramService) =>
    await ramService.GetRamOffers(id).ToListAsync());

app.MapGet("/motherboard", async (string? socket, IMotherboardService motherboardService) =>
    await motherboardService.GetMotherboards(socket).ToListAsync());
app.MapGet("/motherboard/{id}", async (Guid id, IMotherboardService motherboardService) =>
{
    var motherboard = await motherboardService.GetMotherboard(id);
    return motherboard is null ? Results.NotFound() : Results.Ok(motherboard);
});
app.MapGet("/motherboard/{id}/offers", async (Guid id, IMotherboardService motherboardService) =>
    await motherboardService.GetMotherboardOffers(id).ToListAsync());

app.MapGet("/cooler", async (string? socket, ICoolerService coolerService) =>
    await coolerService.GetCoolers(socket).ToListAsync());
app.MapGet("/cooler/{id}", async (Guid id, ICoolerService coolerService) =>
{
    var cooler = await coolerService.GetCooler(id);
    return cooler is null ? Results.NotFound() : Results.Ok(cooler);
});
app.MapGet("/cooler/{id}/offers", async (Guid id, ICoolerService coolerService) =>
    await coolerService.GetCoolerOffers(id).ToListAsync());

app.MapGet("/gpu", async (IGpuService gpuService) => await gpuService.GetGpus().ToListAsync());
app.MapGet("/gpu/{id}", async (Guid id, IGpuService gpuService) =>
{
    var gpu = await gpuService.GetGpu(id);
    return gpu is null ? Results.NotFound() : Results.Ok(gpu);
});
app.MapGet("/gpu/{id}/offers", async (Guid id, IGpuService gpuService) =>
    await gpuService.GetGpuOffers(id).ToListAsync());

app.MapGet("/drive", async (string? type, int? capacity, IInternalDriveService internalDriveService) =>
    await internalDriveService.GetInternalDrives(type, capacity).ToListAsync());
app.MapGet("/drive/{id}", async (Guid id, IInternalDriveService internalDriveService) =>
{
    var internalDrive = await internalDriveService.GetInternalDrive(id);
    return internalDrive is null ? Results.NotFound() : Results.Ok(internalDrive);
});
app.MapGet("/drive/{id}/offers", async (Guid id, IInternalDriveService internalDriveService) =>
    await internalDriveService.GetInternalDriveOffers(id).ToListAsync());

app.MapGet("/psu", async (int? minPower, IPsuService psuService) =>
    await psuService.GetPsus(minPower).ToListAsync());
app.MapGet("/psu/{id}", async (Guid id, IPsuService psuService) =>
{
    var psu = await psuService.GetPsu(id);
    return psu is null ? Results.NotFound() : Results.Ok(psu);
});
app.MapGet("/psu/{id}/offers", async (Guid id, IPsuService psuService) =>
    await psuService.GetPsuOffers(id).ToListAsync());

app.MapGet("/benchmarks/cpu/byId/{id}", async (Guid id, ICpuBenchmarkService cpuBenchmarkService) =>
{
    var cpuBenchmark = await cpuBenchmarkService.GetCpuBenchmark(id);
    return cpuBenchmark is null ? Results.NotFound() : Results.Ok(cpuBenchmark);
});
app.MapGet("/benchmarks/cpu/byName/{name}", async (string name, ICpuBenchmarkService cpuBenchmarkService) =>
{
    var cpuBenchmark = await cpuBenchmarkService.GetCpuBenchmark(name);
    return cpuBenchmark is null ? Results.NotFound() : Results.Ok(cpuBenchmark);
});
app.MapGet("/benchmarks/gpu/byId/{id}", async (Guid id, IGpuBenchmarkService gpuBenchmarkService) =>
{
    var gpuBenchmark = await gpuBenchmarkService.GetGpuBenchmark(id);
    return gpuBenchmark is null ? Results.NotFound() : Results.Ok(gpuBenchmark);
});
app.MapGet("/benchmarks/gpu/byName/{name}", async (string name, IGpuBenchmarkService gpuBenchmarkService) =>
{
    var gpuBenchmark = await gpuBenchmarkService.GetGpuBenchmark(name);
    return gpuBenchmark is null ? Results.NotFound() : Results.Ok(gpuBenchmark);
});

app.MapGet("/pc/check", async (PcDtoModel pc, IPcCheckService pcCheckService) =>
    await pcCheckService.CheckPc(pc));
app.MapGet("/pc/compare/requirements", async (PcDtoModel pc, SystemRequirementsDtoModel systemRequirements, IPcCompareService pcCompareService) =>
    await pcCompareService.IsFitRequirements(pc, systemRequirements));
app.MapGet("/pc/compare/pc", async (PcDtoModel a, PcDtoModel b, IPcCompareService pcCompareService) =>
    await pcCompareService.ComparePcs(a, b));

app.Run();