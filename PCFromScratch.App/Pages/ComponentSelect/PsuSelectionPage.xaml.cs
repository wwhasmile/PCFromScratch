using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class PsuSelectionPage : ContentPage
{
    public PsuSelectionPage(PsuSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}