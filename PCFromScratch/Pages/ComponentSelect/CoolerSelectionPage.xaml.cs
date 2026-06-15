using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class CoolerSelectionPage : ContentPage
{
    public CoolerSelectionPage(CoolerSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}