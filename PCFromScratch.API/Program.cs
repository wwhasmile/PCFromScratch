using Microsoft.EntityFrameworkCore;

using PCFromScratch.API.Services;
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
builder.Services.AddScoped<IGpuRepository, StorageGpuRepository>();
builder.Services.AddScoped<IInternalDriveRepository, StorageInternalDriveRepository>();
builder.Services.AddScoped<ICpuService, CpuService>();
builder.Services.AddScoped<IGpuService, GpuService>();
builder.Services.AddScoped<IInternalDriveService, InternalDriveService>();

builder.Services.AddHostedService<CpuScraperBackgroundService>();
builder.Services.AddHostedService<GpuScraperBackgroundService>();
builder.Services.AddHostedService<HddScraperBackgroundService>();
builder.Services.AddHostedService<SsdScraperBackgroundService>();

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

app.Run();