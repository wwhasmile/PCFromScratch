using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

[QueryProperty(nameof(SelectedPartId), "SelectedPartId")]
[QueryProperty(nameof(Category), "Category")]
public partial class CanIRunOnItPage : ContentPage
{
    public Guid SelectedPartId { get; set; }
    public string Category { get; set; }

    public CanIRunOnItPage(CanIRunOnItViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (SelectedPartId != Guid.Empty && !string.IsNullOrEmpty(Category))
        {
            var viewModel = (CanIRunOnItViewModel)BindingContext;
            await viewModel.UpdateBenchmark(Category, SelectedPartId);

            // Clear properties to prevent re-processing
            SelectedPartId = Guid.Empty;
            Category = null;
        }
    }
}