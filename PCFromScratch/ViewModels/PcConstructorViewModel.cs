using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PCFromScratch.App.Pages;
using PCFromScratch.App.Utils;
using PCFromScratch.Common;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

public class PcConstructorViewModel : INotifyPropertyChanged
{
    private readonly ServerRequests _serverRequests;

    private PcDtoModel _pc;
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
    public ICommand GoToCanIRunOnItPageCommand { get; }
    public ICommand OpenLinkCommand { get; }

    public PcConstructorViewModel(ServerRequests serverRequests)
    {
        Components = new ObservableCollection<BaseComponentCategory>
        {
            new SingleComponentCategory("Процесор (CPU)"),
            new SingleComponentCategory("Охолодження процесора"),
            new SingleComponentCategory("Материнська плата"),
            new SingleComponentCategory("Оперативна пам'ять"),
            new MultiComponentCategory("Накопичувач"),
            new SingleComponentCategory("Відеокарта (GPU)"),
            new SingleComponentCategory("Блок живлення")
        };
        
        Warnings = new ObservableCollection<Warning>();

        foreach (var component in Components)
        {
            component.PropertyChanged += (s, e) => RecalculateTotal();
            if (component is MultiComponentCategory multi)
            {
                multi.SelectedParts.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (Part item in e.NewItems)
                        {
                            item.PropertyChanged += Part_PropertyChanged;
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (Part item in e.OldItems)
                        {
                            item.PropertyChanged -= Part_PropertyChanged;
                        }
                    }
                    RecalculateTotal();
                };
            }
        }
        _serverRequests = serverRequests;

        ChooseCommand = new AsyncRelayCommand<BaseComponentCategory>(OnChoose);
        RemoveCommand = new Command<Part>(OnRemove);
        CheckCompatibilityCommand = new AsyncRelayCommand(CheckCompatibility);
        GoToCanIRunOnItPageCommand = new AsyncRelayCommand(GoToCanIRunOnItPage);
        OpenLinkCommand = new AsyncRelayCommand<string>(OpenLink);

        _pc = new PcDtoModel();
        _pc.InternalDrives = [];
    }
    
    private async Task OpenLink(string? url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            await Launcher.OpenAsync(url);
        }
    }
    
    private void Part_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Part.SelectedOffer))
        {
            RecalculateTotal();
        }
    }

    private async Task OnChoose(BaseComponentCategory? component)
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
        if (_pc.InternalDrives.Contains(partToRemove.Id))
        {
            _pc.InternalDrives.Remove(partToRemove.Id);
        }
    }

    public async Task UpdateSelectedComponent(string category, Guid selectedPartId)
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
        
        string name = "", link = "";
        string? image = null;

        switch (category)
        {
            case "Cpu":
                var cpu = await _serverRequests.GetItem<CpuDtoModel>("/cpu", selectedPartId);
                name = cpu.Name;
                image = cpu.Image;
                link = cpu.Link;
                break;
            case "Cooler":
                var cooler = await _serverRequests.GetItem<CoolerDtoModel>("/cooler", selectedPartId);
                name = cooler.Name;
                image = cooler.ImageUrl;
                link = cooler.Link;
                break;
            case "Motherboard":
                var motherboard = await _serverRequests.GetItem<MotherboardDtoModel>("/motherboard", selectedPartId);
                name = motherboard.Name;
                image = motherboard.ImageUrl;
                link = motherboard.Link;
                break;
            case "Ram":
                var ram = await _serverRequests.GetItem<RamDtoModel>("/ram", selectedPartId);
                name = ram.Model;
                image = ram.Image;
                link = ram.Link;
                break;
            case "Storage":
                var storage = await _serverRequests.GetItem<InternalDriveDtoModel>("/drive", selectedPartId);
                name = storage.Name;
                image = storage.Image;
                link = storage.Link;
                break;
            case "Gpu":
                var gpu = await _serverRequests.GetItem<GpuDtoModel>("/gpu", selectedPartId);
                name = gpu.Name;
                image = gpu.Image;
                link = gpu.Link;
                break;
            case "Psu":
                var psu = await _serverRequests.GetItem<PsuDtoModel>("/psu", selectedPartId);
                name = psu.Name;
                image = psu.Image;
                link = psu.Link;
                break;
        }

        if (name == string.Empty) return;

        IEnumerable<OfferDtoModel>? offers = category switch
        {
            "Cpu" => await _serverRequests.GetOffers("/cpu", selectedPartId),
            "Gpu" => await _serverRequests.GetOffers("/gpu", selectedPartId),
            "Cooler" => await _serverRequests.GetOffers("/cooler", selectedPartId),
            "Motherboard" => await _serverRequests.GetOffers("/motherboard", selectedPartId),
            "Ram" => await _serverRequests.GetOffers("/ram", selectedPartId),
            "Storage" => await _serverRequests.GetOffers("/drive", selectedPartId),
            "Psu" => await _serverRequests.GetOffers("/psu", selectedPartId),
            _ => null
        };
        var component = Components.FirstOrDefault(c => c.Name == categoryName);
        if (component is SingleComponentCategory single)
        {
            single.SelectedPart = new Part(selectedPartId, name, image, link, offers ?? new List<OfferDtoModel>());
        }
        else if (component is MultiComponentCategory multi)
        {
            multi.SelectedParts.Add(new Part(selectedPartId, name, image, link, offers ?? new List<OfferDtoModel>()));
        }
        switch (category)
        {
            case "Cpu":
                _pc.Cpu = selectedPartId;
                break;
            case "Cooler":
                _pc.Cooler = selectedPartId;
                break;
            case "Motherboard":
                _pc.Motherboard = selectedPartId;
                break;
            case "Ram":
                _pc.Ram = selectedPartId;
                break;
            case "Storage":
                _pc.InternalDrives.Add(selectedPartId);
                break;
            case "Gpu":
                _pc.Gpu = selectedPartId;
                break;
            case "Psu":
                _pc.Psu = selectedPartId;
                break;
        }
    }

    private void RecalculateTotal()
    {
        decimal total = 0;
        foreach (var component in Components)
        {
            if (component is SingleComponentCategory single && single.SelectedOffer != null)
            {
                total += single.SelectedOffer.Value.Price;
            }
            else if (component is MultiComponentCategory multi)
            {
                total += multi.SelectedParts.Sum(p => p.SelectedOffer != null ? p.SelectedOffer.Value.Price : 0);
            }
        }
        TotalCost = total;
    }
    
    private async Task CheckCompatibility()
    {
        var results = await _serverRequests.CheckPc(_pc) ?? new List<Warning>();
        
        Warnings.Clear();
        foreach (var warning in results)
        {
            Warnings.Add(warning);
        }
    }

    private async Task GoToCanIRunOnItPage()
    {
        if (_pc.Cpu == null || _pc.Motherboard == null || _pc.Gpu == null || _pc.Ram == null || _pc.Psu == null || _pc.InternalDrives.Count == 0) return;
        var navigationParameter = new Dictionary<string, object>
        {
            { "pc", _pc }
        };
        await Shell.Current.GoToAsync($"{nameof(CanIRunOnItPage)}", navigationParameter);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}