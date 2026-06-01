using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PCFromScratch.App.Pages;
using PCFromScratch.Common;

namespace PCFromScratch.App.ViewModels;

public class PcConstructorViewModel : INotifyPropertyChanged
{
    public ObservableCollection<BaseComponentCategory> Components { get; set; }
    public ObservableCollection<Warning> Warnings { get; set; }
    
    public decimal TotalCost
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public ICommand ChooseCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand CheckCompatibilityCommand { get; }

    public PcConstructorViewModel()
    {
        Components = new ObservableCollection<BaseComponentCategory>
        {
            new SingleComponentCategory("Процесор (CPU)"),
            new SingleComponentCategory("Охолодження процесора"),
            new SingleComponentCategory("Материнська плата"),
            new SingleComponentCategory("Оперативна пам'ять"),
            new MultiComponentCategory("Накопичувач"),
            new SingleComponentCategory("Відеокарта (GPU)"),
            new SingleComponentCategory("Блок живлення"),
            new SingleComponentCategory("Корпус")
        };
        
        Warnings = new ObservableCollection<Warning>();

        foreach (var component in Components)
        {
            component.PropertyChanged += (s, e) => RecalculateTotal();
            if (component is MultiComponentCategory multi)
            {
                multi.SelectedParts.CollectionChanged += (s, e) => RecalculateTotal();
            }
        }

        ChooseCommand = new Command<BaseComponentCategory>(OnChoose);
        RemoveCommand = new Command<Part>(OnRemove);
        CheckCompatibilityCommand = new Command(CheckCompatibility);
    }

    private async void OnChoose(BaseComponentCategory component)
    {
        if (component != null)
        {
            var pageName = component.Name switch
            {
                "Процесор (CPU)" => nameof(CpuSelectionPage),
                "Охолодження процесора" => nameof(CoolerSelectionPage),
                "Материнська плата" => nameof(MotherboardSelectionPage),
                "Оперативна пам'ять" => nameof(RamSelectionPage),
                "Накопичувач" => nameof(StorageSelectionPage),
                "Відеокарта (GPU)" => nameof(GpuSelectionPage),
                "Блок живлення" => nameof(PsuSelectionPage),
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(pageName))
            {
                await Shell.Current.GoToAsync(pageName);
            }
        }
    }

    private void OnRemove(Part? partToRemove)
    {
        if (partToRemove is null) return;
        foreach (var component in Components.OfType<MultiComponentCategory>())
        {
            if (component.SelectedParts.Contains(partToRemove))
            {
                component.SelectedParts.Remove(partToRemove);
                break;
            }
        }
    }

    public void UpdateSelectedComponent(string category, ComponentModel selectedPart)
    {
        var categoryName = category switch
        {
            "Cpu" => "Процесор (CPU)",
            "Cooler" => "Охолодження процесора",
            "Motherboard" => "Материнська плата",
            "Ram" => "Оперативна пам'ять",
            "Storage" => "Накопичувач",
            "Gpu" => "Відеокарта (GPU)",
            "Psu" => "Блок живлення",
            _ => string.Empty
        };
        var component = Components.FirstOrDefault(c => c.Name == categoryName);
        if (component is SingleComponentCategory single)
        {
            single.SelectedPart = new Part(selectedPart.Name, new List<Offer> { new("Rozetka", 100), new("Comfy", 200) });
        }
        else if (component is MultiComponentCategory multi)
        {
            multi.SelectedParts.Add(new Part(selectedPart.Name, new List<Offer> { new("Rozetka", 100), new("Comfy", 200) }));
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
                total += multi.SelectedParts.Sum(p => p.SelectedOffer != null ? p.SelectedOffer.Price : 0);
            }
        }
        TotalCost = total;
    }
    
    private void CheckCompatibility()
    {
        Warnings.Clear();
        // This is a mock implementation. In the future, this will be replaced with a call to a compatibility check service.
        Warnings.Add(new Warning(WarningSeverity.Incompatibility, "Процесор і материнська плата мають різні сокети."));
        Warnings.Add(new Warning(WarningSeverity.Bottleneck, "Материнська плата підтримує максимальну частоту пам'яті N МГц, обрана па'мять має частоту K МГц."));
        Warnings.Add(new Warning(WarningSeverity.Info, "У BOX комплектації процесора кулер йде у комплекті, рекомендовано замінити комплектацію процесора, або прибрати систему охолодження"));
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}