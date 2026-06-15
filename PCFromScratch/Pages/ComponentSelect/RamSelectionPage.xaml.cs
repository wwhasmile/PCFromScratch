using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class RamSelectionPage : ContentPage
{
    public RamSelectionPage(RamSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}