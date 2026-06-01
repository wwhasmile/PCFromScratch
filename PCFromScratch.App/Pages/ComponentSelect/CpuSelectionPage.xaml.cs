using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class CpuSelectionPage : ContentPage
{
    public CpuSelectionPage(CpuSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}