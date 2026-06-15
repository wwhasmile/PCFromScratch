using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PCFromScratch.App.ViewModels;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.Pages;

[QueryProperty(nameof(SelectedPartId), "SelectedPartId")]
[QueryProperty(nameof(Category), "Category")]
public partial class PcConstructorPage
{
    public Guid SelectedPartId { get; set; }
    public string? Category { get; set; }

    public PcConstructorPage(PcConstructorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (SelectedPartId != Guid.Empty && !string.IsNullOrEmpty(Category))
        {
            var viewModel = (PcConstructorViewModel)BindingContext;
            _ = viewModel.UpdateSelectedComponent(Category, SelectedPartId);

            // Clear properties to prevent re-processing
            SelectedPartId = Guid.Empty;
            Category = null;
        }
    }
}

public abstract class BaseComponentCategory (string name) : INotifyPropertyChanged
{
    public string Name { get; set; } = name;
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class SingleComponentCategory (string name): BaseComponentCategory (name)
{
    public Part? SelectedPart
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonText));
                SelectedOffer = field?.Offers.FirstOrDefault();
            }
        }
    }
    
    public OfferDtoModel? SelectedOffer
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }
    public string ButtonText => SelectedPart == null ? "Обрати" : "Змінити";
}

public class MultiComponentCategory (string name): BaseComponentCategory (name)
{
    public ObservableCollection<Part> SelectedParts { get; } = new();
}

public class ComponentCategoryTemplateSelector : DataTemplateSelector
{
    public DataTemplate SingleComponentCategoryTemplate { get; set; } = null!;
    public DataTemplate MultiComponentCategoryTemplate { get; set; } = null!;

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return item switch
        {
            SingleComponentCategory => SingleComponentCategoryTemplate,
            MultiComponentCategory => MultiComponentCategoryTemplate,
            _ => SingleComponentCategoryTemplate
        };
    }

}