using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class MotherboardSelectionPage : ContentPage
{
    public MotherboardSelectionPage(MotherboardSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}