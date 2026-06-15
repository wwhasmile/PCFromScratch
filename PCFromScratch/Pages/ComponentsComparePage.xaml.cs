using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class ComponentsComparePage : ContentPage
{
    public ComponentsComparePage(ComponentsCompareViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var viewModel = (ComponentsCompareViewModel)BindingContext;
        if (viewModel.SelectedPartId != null)
        {
            await viewModel.UpdateSelectedComponent();
        }
    }
}