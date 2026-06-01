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
    }
}