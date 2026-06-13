using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

[QueryProperty(nameof(SelectedPartId), "SelectedPartId")]
[QueryProperty(nameof(Category), "Category")]
[QueryProperty(nameof(PcIndex), "PcIndex")]
public partial class PcComparePage : ContentPage
{
    public Guid SelectedPartId { get; set; }
    public string Category { get; set; }
    public int PcIndex { get; set; }

    public PcComparePage(PcCompareViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (SelectedPartId != Guid.Empty && !string.IsNullOrEmpty(Category) && PcIndex != 0)
        {
            var viewModel = (PcCompareViewModel)BindingContext;
            await viewModel.UpdateComponent(Category, SelectedPartId, PcIndex);

            // Clear properties to prevent re-processing
            SelectedPartId = Guid.Empty;
            Category = null;
            PcIndex = 0;
        }
    }

    private async void OnSelectComponent(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not string component) return;
        
        var pcIndex = component.StartsWith("Pc1") ? 1 : 2;
        var category = component.Replace("Pc1", "").Replace("Pc2", "");
        
        var pageName = category switch
        {
            "Cpu" => "CpuSelectionPage",
            "Gpu" => "GpuSelectionPage",
            "Ram" => "RamSelectionPage",
            "Psu" => "PsuSelectionPage",
            "Storage" => "StorageSelectionPage",
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(pageName))
        {
            await Shell.Current.GoToAsync($"{pageName}?PcIndex={pcIndex}");
        }
    }
    
    private void OnCompareClicked(object sender, EventArgs e)
    {
        var viewModel = (PcCompareViewModel)BindingContext;
        viewModel.CompareCommand.Execute(null);
    }
}