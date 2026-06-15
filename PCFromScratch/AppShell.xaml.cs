using PCFromScratch.App.Pages;

namespace PCFromScratch.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute(nameof(CpuSelectionPage), typeof(CpuSelectionPage));
        Routing.RegisterRoute(nameof(CoolerSelectionPage), typeof(CoolerSelectionPage));
        Routing.RegisterRoute(nameof(MotherboardSelectionPage), typeof(MotherboardSelectionPage));
        Routing.RegisterRoute(nameof(RamSelectionPage), typeof(RamSelectionPage));
        Routing.RegisterRoute(nameof(StorageSelectionPage), typeof(StorageSelectionPage));
        Routing.RegisterRoute(nameof(GpuSelectionPage), typeof(GpuSelectionPage));
        Routing.RegisterRoute(nameof(PsuSelectionPage), typeof(PsuSelectionPage));
        Routing.RegisterRoute(nameof(CpuBenchmarkSelectionPage), typeof(CpuBenchmarkSelectionPage));
        Routing.RegisterRoute(nameof(GpuBenchmarkSelectionPage), typeof(GpuBenchmarkSelectionPage));
        Routing.RegisterRoute(nameof(CanIRunOnItPage), typeof(CanIRunOnItPage));
        Routing.RegisterRoute(nameof(PcComparePage), typeof(PcComparePage));
    }
}