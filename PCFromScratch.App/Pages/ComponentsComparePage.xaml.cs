using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class ComponentsComparePage : ContentPage
{
    public ComponentsComparePage(ComponentsCompareViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}