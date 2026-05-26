using PCFromScratch.App.Pages;

namespace PCFromScratch.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute(nameof(ComponentSelectionPage), typeof(ComponentSelectionPage));
    }
}
