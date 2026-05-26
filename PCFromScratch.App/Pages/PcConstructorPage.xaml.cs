using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PCFromScratch.App.Pages;

public partial class PcConstructorPage : ContentPage, INotifyPropertyChanged
{
    public ObservableCollection<BaseComponentCategory> Components { get; set; }

    private decimal _totalCost;
    public decimal TotalCost
    {
        get => _totalCost;
        set
        {
            _totalCost = value;
            OnPropertyChanged();
        }
    }

    public PcConstructorPage()
    {
        InitializeComponent();
        BindingContext = this;

        Components = new ObservableCollection<BaseComponentCategory>
        {
            new SingleComponentCategory { Name = "Процесор (CPU)" },
            new SingleComponentCategory { Name = "Охолодження процесора" },
            new SingleComponentCategory { Name = "Материнська плата" },
            new SingleComponentCategory { Name = "Оперативна пам'ять" },
            new MultiComponentCategory { Name = "Накопичувач" },
            new SingleComponentCategory { Name = "Відеокарта (GPU)" },
            new SingleComponentCategory { Name = "Блок живлення" },
            new SingleComponentCategory { Name = "Корпус" }
        };

        foreach (var component in Components)
        {
            component.PropertyChanged += (s, e) => RecalculateTotal();
            if (component is MultiComponentCategory multi)
            {
                multi.SelectedParts.CollectionChanged += (s, e) => RecalculateTotal();
            }
        }

        ComponentCollectionView.ItemsSource = Components;
    }

    private async void ChooseButton_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        if (button.CommandParameter is BaseComponentCategory component)
        {
            await Shell.Current.GoToAsync($"{nameof(ComponentSelectionPage)}?Category={component.Name}");
        }
    }
    
    private void RemovePart_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        if (button.CommandParameter is Part partToRemove)
        {
            foreach (var component in Components.OfType<MultiComponentCategory>())
            {
                if (component.SelectedParts.Contains(partToRemove))
                {
                    component.SelectedParts.Remove(partToRemove);
                    break;
                }
            }
        }
    }

    private void RecalculateTotal()
    {
        decimal total = 0;
        foreach (var component in Components)
        {
            if (component is SingleComponentCategory single && single.SelectedOffer != null)
            {
                total += single.SelectedOffer.Price;
            }
            else if (component is MultiComponentCategory multi)
            {
                total += multi.SelectedParts.Where(p => p.SelectedOffer != null).Sum(p => p.SelectedOffer.Price);
            }
        }
        TotalCost = total;
    }
    
    public new event PropertyChangedEventHandler PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public abstract class BaseComponentCategory : INotifyPropertyChanged
{
    public string Name { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class SingleComponentCategory : BaseComponentCategory
{
    private Part _selectedPart;
    public Part SelectedPart
    {
        get => _selectedPart;
        set
        {
            if (_selectedPart != value)
            {
                _selectedPart = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonText));
                SelectedOffer = _selectedPart?.Offers.FirstOrDefault();
            }
        }
    }

    private Offer _selectedOffer;
    public Offer SelectedOffer
    {
        get => _selectedOffer;
        set
        {
            if (_selectedOffer != value)
            {
                _selectedOffer = value;
                OnPropertyChanged();
            }
        }
    }
    public string ButtonText => SelectedPart == null ? "Обрати" : "Змінити";
}

public class MultiComponentCategory : BaseComponentCategory
{
    public ObservableCollection<Part> SelectedParts { get; } = new();
}
