using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class StorageSelectionPage : ContentPage
{
    public StorageSelectionPage(StorageSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}