using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

[QueryProperty(nameof(Category), "Category")]
public partial class ComponentSelectionPage : ContentPage
{
    private string _category;
    public string Category
    {
        get => _category;
        set
        {
            _category = value;
            var viewModel = (ComponentSelectionViewModel)BindingContext;
            if (viewModel != null)
            {
                viewModel.Category = _category;
            }
        }
    }

    public ComponentSelectionPage()
    {
        InitializeComponent();
        BindingContext = new ComponentSelectionViewModel();
    }
}