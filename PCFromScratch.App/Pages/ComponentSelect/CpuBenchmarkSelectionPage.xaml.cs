using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class CpuBenchmarkSelectionPage : ContentPage
{
    public CpuBenchmarkSelectionPage(CpuBenchmarkSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}