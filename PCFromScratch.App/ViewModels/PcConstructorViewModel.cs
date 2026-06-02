using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using PCFromScratch.App.Pages;
using PCFromScratch.Common;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;
using PCFromScratch.Services;

namespace PCFromScratch.App.ViewModels;

public class PcConstructorViewModel : INotifyPropertyChanged
{
    private readonly ICpuRepository _cpuRepository;
    private readonly ICoolerRepository _coolerRepository;
    private readonly IMotherboardRepository _motherboardRepository;
    private readonly IRamRepository _ramRepository;
    private readonly IInternalDriveRepository _storageRepository;
    private readonly IGpuRepository _gpuRepository;
    private readonly IPsuRepository _psuRepository;
    private readonly IPcCheckService _pcCheckService;

    public PcDtoModel Pc { get; set; }
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

    public PcConstructorViewModel(ICpuRepository cpuRepository, ICoolerRepository coolerRepository, 
        IMotherboardRepository motherboardRepository, IRamRepository ramRepository, 
        IInternalDriveRepository storageRepository, IGpuRepository gpuRepository, IPsuRepository psuRepository,
        IPcCheckService pcCheckService)
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

        _cpuRepository = cpuRepository;
        _coolerRepository = coolerRepository;
        _motherboardRepository = motherboardRepository;
        _ramRepository = ramRepository;
        _storageRepository = storageRepository;
        _gpuRepository = gpuRepository;
        _psuRepository = psuRepository;
        _pcCheckService = pcCheckService;

        ChooseCommand = new AsyncRelayCommand<BaseComponentCategory>(OnChoose);
        RemoveCommand = new Command<Part>(OnRemove);
        CheckCompatibilityCommand = new AsyncRelayCommand(CheckCompatibility);

        Pc = new PcDtoModel();
    }
    
    private void Part_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Part.SelectedOffer))
        {
            RecalculateTotal();
        }
    }

    private async Task OnChoose(BaseComponentCategory component)
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
        if (Pc.InternalDrives.Contains(partToRemove.Id))
        {
            Pc.InternalDrives.Remove(partToRemove.Id);
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

        string name = category switch
        {
            "Cpu" => (await _cpuRepository.GetCpu(selectedPartId)).Name,
            "Cooler" => (await _coolerRepository.GetCooler(selectedPartId)).Name,
            "Motherboard" => (await _motherboardRepository.GetMotherboard(selectedPartId)).Name,
            "Ram" => (await _ramRepository.GetRam(selectedPartId)).Name,
            "Storage" => (await _storageRepository.GetInternalDrive(selectedPartId)).Name,
            "Gpu" => (await _gpuRepository.GetGpu(selectedPartId)).Name,
            "Psu" => (await _psuRepository.GetPsu(selectedPartId)).Name,
            _ => string.Empty
        };

        if (name == null) return;

        var component = Components.FirstOrDefault(c => c.Name == categoryName);
        if (component is SingleComponentCategory single)
        {
            single.SelectedPart = new Part(selectedPartId, name, new List<Offer> { new("Rozetka", 100), new("Comfy", 200) });
        }
        else if (component is MultiComponentCategory multi)
        {
            multi.SelectedParts.Add(new Part(selectedPartId, name, new List<Offer> { new("Rozetka", 100), new("Comfy", 200) }));
        }
        switch (category)
        {
            case "Cpu":
                Pc.Cpu = selectedPartId;
                break;
            case "Cooler":
                Pc.Cooler = selectedPartId;
                break;
            case "Motherboard":
                Pc.Motherboard = selectedPartId;
                break;
            case "Ram":
                Pc.Ram = selectedPartId;
                break;
            case "Storage":
                Pc.InternalDrives.Add(selectedPartId);
                break;
            case "Gpu":
                Pc.Gpu = selectedPartId;
                break;
            case "Psu":
                Pc.Psu = selectedPartId;
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
                total += single.SelectedOffer.Price;
            }
            else if (component is MultiComponentCategory multi)
            {
                total += multi.SelectedParts.Sum(p => p.SelectedOffer != null ? p.SelectedOffer.Price : 0);
            }
        }
        TotalCost = total;
    }
    
    private async Task CheckCompatibility()
    {
        var results = await _pcCheckService.CheckPc(Pc);
        
        Warnings.Clear();
        foreach (var warning in results)
        {
            Warnings.Add(warning);
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}