using Microsoft.EntityFrameworkCore;

using PCFromScratch.API;
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
builder.Services.AddScoped<ICpuService, CpuService>();

builder.Services.AddHostedService<CpuScraperBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/cpu", async (ICpuService cpuService) => await cpuService.GetCpus().ToListAsync());
app.MapGet("/cpu/{id}", async (Guid id, ICpuService cpuService) =>
{
    var cpu = await cpuService.GetCpu(id);
    return cpu is null ? Results.NotFound() : Results.Ok(cpu);
});
app.MapGet("/cpu/{id}/offers", async (Guid id, ICpuService cpuService) =>
    await cpuService.GetCpuOffers(id).ToListAsync());

app.Run();