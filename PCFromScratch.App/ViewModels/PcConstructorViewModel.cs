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
            new SingleComponentCategory { Name = "Процесор (CPU)" },
            new SingleComponentCategory { Name = "Охолодження процесора" },
            new SingleComponentCategory { Name = "Материнська плата" },
            new SingleComponentCategory { Name = "Оперативна пам'ять" },
            new MultiComponentCategory { Name = "Накопичувач" },
            new SingleComponentCategory { Name = "Відеокарта (GPU)" },
            new SingleComponentCategory { Name = "Блок живлення" },
            new SingleComponentCategory { Name = "Корпус" }
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
            await Shell.Current.GoToAsync($"{nameof(ComponentSelectionPage)}?Category={component.Name}");
        }
    }

    private void OnRemove(Part partToRemove)
    {
        if (partToRemove != null)
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
    
    private void CheckCompatibility()
    {
        Warnings.Clear();
        // This is a mock implementation. In the future, this will be replaced with a call to a compatibility check service.
        Warnings.Add(new Warning(WarningSeverity.Incompatibility, "Процесор і материнська плата мають різні сокети."));
        Warnings.Add(new Warning(WarningSeverity.Bottleneck, "Материнська плата підтримує максимальну частоту пам'яті N МГц, обрана па'мять має частоту K МГц."));
        Warnings.Add(new Warning(WarningSeverity.Info, "У BOX комплектації процесора кулер йде у комплекті, рекомендовано замінити комплектацію процесора, або прибрати систему охолодження"));
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}