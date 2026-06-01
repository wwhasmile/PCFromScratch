using Microsoft.Extensions.Logging;
using PCFromScratch.App.Pages;
using PCFromScratch.App.ViewModels;
using PCFromScratch.Repository;
using PCFromScratch.Storage;

namespace PCFromScratch.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<IStorageContext, EntityStorageContext>();
        
        builder.Services.AddSingleton<ICpuRepository, FakeCpu>();
        builder.Services.AddSingleton<IGpuRepository, FakeGpu>();
        builder.Services.AddSingleton<ICoolerRepository, StorageCoolerRepository>();
        builder.Services.AddSingleton<IMotherboardRepository, StorageMotherboardRepository>();
        builder.Services.AddSingleton<IRamRepository, StorageRamRepository>();
        builder.Services.AddSingleton<IInternalDriveRepository, StorageInternalDriveRepository>();
        builder.Services.AddSingleton<IPsuRepository, StoragePsuRepository>();

        builder.Services.AddSingleton<ComponentsCompareViewModel>();
        builder.Services.AddSingleton<PcConstructorViewModel>();
        builder.Services.AddTransient<CpuSelectionViewModel>();
        builder.Services.AddTransient<CoolerSelectionViewModel>();
        builder.Services.AddTransient<MotherboardSelectionViewModel>();
        builder.Services.AddTransient<RamSelectionViewModel>();
        builder.Services.AddTransient<StorageSelectionViewModel>();
        builder.Services.AddTransient<GpuSelectionViewModel>();
        builder.Services.AddTransient<PsuSelectionViewModel>();

        builder.Services.AddSingleton<ComponentsComparePage>();
        builder.Services.AddSingleton<PcConstructorPage>();
        builder.Services.AddTransient<CpuSelectionPage>();
        builder.Services.AddTransient<CoolerSelectionPage>();
        builder.Services.AddTransient<MotherboardSelectionPage>();
        builder.Services.AddTransient<RamSelectionPage>();
        builder.Services.AddTransient<StorageSelectionPage>();
        builder.Services.AddTransient<GpuSelectionPage>();
        builder.Services.AddTransient<PsuSelectionPage>();

        return builder.Build();
    }
}