using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class GpuBenchmarkSelectionPage : ContentPage
{
    public GpuBenchmarkSelectionPage(GpuBenchmarkSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}