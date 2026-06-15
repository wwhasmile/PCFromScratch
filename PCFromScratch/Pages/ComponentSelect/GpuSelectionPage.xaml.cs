using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class GpuSelectionPage : ContentPage
{
    public GpuSelectionPage(GpuSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}