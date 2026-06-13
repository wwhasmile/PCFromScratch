using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

[QueryProperty(nameof(Pc), "pc")]
public partial class CanIRunOnItViewModel : ObservableObject
{
    private readonly ServerRequests _requests;

    [ObservableProperty]
    private PcDtoModel _pc;

    [ObservableProperty]
    private string _cpuName;
    [ObservableProperty]
    private string _gpuName;
    [ObservableProperty]
    private string _ramName;
    [ObservableProperty]
    private string _psuName;
    [ObservableProperty]
    private string _storageNames;

    [ObservableProperty]
    private SystemRequirementsDtoModel _requirements;

    [ObservableProperty]
    private string _resultMessage;
    
    [ObservableProperty]
    private string _selectedCpuBenchmarkName;
    
    [ObservableProperty]
    private string _selectedGpuBenchmarkName;

    [ObservableProperty]
    private int? _ramInMegabytes;

    [ObservableProperty]
    private int? _spaceOnDiskInGigabytes;

    public CanIRunOnItViewModel(ServerRequests requests)
    {
        _requests = requests;
        _requirements = new SystemRequirementsDtoModel();
        SelectedCpuBenchmarkName = "Процесор";
        SelectedGpuBenchmarkName = "Відеокарта";
    }

    public async Task UpdateBenchmark(string category, Guid id)
    {
        var reqs = Requirements;
        if (category == "CpuBenchmark")
        {
            reqs.CpuBenchmark = id;
            var benchmark = await _requests.GetItem<CpuBenchmarkDtoModel>("/benchmarks/cpu/byId", id);
            if (benchmark != null)
            {
                SelectedCpuBenchmarkName = benchmark.Name;
            }
        }
        else if (category == "GpuBenchmark")
        {
            reqs.GpuBenchmark = id;
            var benchmark = await _requests.GetItem<GpuBenchmarkDtoModel>("/benchmarks/gpu/byId", id);
            if (benchmark != null)
            {
                SelectedGpuBenchmarkName = benchmark.Name;
            }
        }
        Requirements = reqs;
    }

    async partial void OnPcChanged(PcDtoModel value)
    {
        if (value.Cpu != Guid.Empty && value.Cpu != null)
            CpuName = (await _requests.GetItem<CpuDtoModel>("/cpu", value.Cpu.Value)).Name;
        if (value.Gpu != Guid.Empty && value.Gpu != null)
            GpuName = (await _requests.GetItem<GpuDtoModel>("/gpu", value.Gpu.Value)).Name;
        if (value.Ram != Guid.Empty && value.Ram != null)
            RamName = (await _requests.GetItem<RamDtoModel>("/ram", value.Ram.Value)).Model;
        if (value.Psu != Guid.Empty && value.Psu != null)
            PsuName = (await _requests.GetItem<PsuDtoModel>("/psu", value.Psu.Value)).Name;

        var storageNames = new List<string>();
        foreach (var driveId in value.InternalDrives)
        {
            var drive = await _requests.GetItem<InternalDriveDtoModel>("/drive", driveId);
            if (drive != null)
            {
                storageNames.Add(drive.Name);
            }
        }
        StorageNames = string.Join(", ", storageNames);
    }

    [RelayCommand]
    private async Task SelectCpuBenchmark()
    {
        await Shell.Current.GoToAsync("CpuBenchmarkSelectionPage");
    }

    [RelayCommand]
    private async Task SelectGpuBenchmark()
    {
        await Shell.Current.GoToAsync("GpuBenchmarkSelectionPage");
    }

    [RelayCommand]
    private async Task CheckRequirements()
    {
        var reqs = Requirements;
        reqs.RamInMegabytes = RamInMegabytes ?? 0;
        reqs.SpaceOnDiskInGigabytes = SpaceOnDiskInGigabytes ?? 0;
        Requirements = reqs;

        if (Requirements.CpuBenchmark is null || Requirements.GpuBenchmark is null || Requirements.RamInMegabytes == 0 || Requirements.SpaceOnDiskInGigabytes == 0)
        {
            ResultMessage = "Оберіть будь ласка усі параметри";
            return;
        }

        var res = await _requests.CheckRequirements(Pc, Requirements);
        if (res is null) return;
        var (isFit, messages) = res.Value;

        if (isFit)
        {
            ResultMessage = "Ваш ПК зможе це запустити!";
        }
        else
        {
            ResultMessage = "Ваш ПК не зможе це запустити. Ось чому:\\n" + string.Join("\\n", messages.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        }
    }
}