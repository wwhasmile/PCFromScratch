using System.Reflection;

using CommunityToolkit.Maui;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PCFromScratch.App.Pages;
using PCFromScratch.App.Utils;
using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("DniproCity-Bold.ttf", "DniproCityBold");
                fonts.AddFont("DniproCity-Regular.ttf", "DniproCityRegular");
            });
        //Register appsettings.json
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.json");

        if (stream != null)
        {
            builder.Configuration.AddJsonStream(stream);
        }
#if DEBUG
        builder.Logging.AddDebug();
#endif
        //Client side requests service
        builder.Services.AddSingleton<ServerRequests>();
        //View models
        builder.Services.AddSingleton<ComponentsCompareViewModel>();
        builder.Services.AddSingleton<PcConstructorViewModel>();
        builder.Services.AddTransient<CpuSelectionViewModel>();
        builder.Services.AddTransient<CoolerSelectionViewModel>();
        builder.Services.AddTransient<MotherboardSelectionViewModel>();
        builder.Services.AddTransient<RamSelectionViewModel>();
        builder.Services.AddTransient<StorageSelectionViewModel>();
        builder.Services.AddTransient<GpuSelectionViewModel>();
        builder.Services.AddTransient<PsuSelectionViewModel>();
        builder.Services.AddTransient<CpuBenchmarkSelectionViewModel>();
        builder.Services.AddTransient<GpuBenchmarkSelectionViewModel>();
        builder.Services.AddTransient<CanIRunOnItViewModel>();
        builder.Services.AddSingleton<PcCompareViewModel>();
        //Pages
        builder.Services.AddSingleton<ComponentsComparePage>();
        builder.Services.AddSingleton<PcConstructorPage>();
        builder.Services.AddTransient<CpuSelectionPage>();
        builder.Services.AddTransient<CoolerSelectionPage>();
        builder.Services.AddTransient<MotherboardSelectionPage>();
        builder.Services.AddTransient<RamSelectionPage>();
        builder.Services.AddTransient<StorageSelectionPage>();
        builder.Services.AddTransient<GpuSelectionPage>();
        builder.Services.AddTransient<PsuSelectionPage>();
        builder.Services.AddTransient<CpuBenchmarkSelectionPage>();
        builder.Services.AddTransient<GpuBenchmarkSelectionPage>();
        builder.Services.AddTransient<CanIRunOnItPage>();
        builder.Services.AddSingleton<PcComparePage>();

        return builder.Build();
    }
}