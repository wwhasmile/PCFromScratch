using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

/// <summary>
/// ViewModel for checking if a PC build can run a game with specific requirements.
/// </summary>
[QueryProperty(nameof(Pc), "pc")]
public partial class CanIRunOnItViewModel : ObservableObject
{
    private readonly ServerRequests _requests;

    [ObservableProperty]
    public partial PcDtoModel Pc { get; set; }

    [ObservableProperty]
    public partial string CpuName { get; set;}
    [ObservableProperty]
    public partial string GpuName { get; set;}
    [ObservableProperty]
    public partial string RamName { get; set;}
    [ObservableProperty]
    public partial string PsuName { get; set;}
    [ObservableProperty]
    public partial string StorageNames { get; set;}

    [ObservableProperty]
    public partial SystemRequirementsDtoModel Requirements { get; set;}

    [ObservableProperty]
    public partial string ResultMessage { get; set;}
    
    [ObservableProperty]
    public partial string SelectedCpuBenchmarkName { get; set;}
    
    [ObservableProperty]
    public partial string SelectedGpuBenchmarkName { get; set;}

    [ObservableProperty]
    public partial int? RamInGigabytes { get; set;}

    [ObservableProperty]
    public partial int? SpaceOnDiskInGigabytes { get; set;}
    
    [ObservableProperty]
    public partial bool SsdRequired { get; set; }

    public CanIRunOnItViewModel(ServerRequests requests)
    {
        _requests = requests;
        Requirements = new SystemRequirementsDtoModel();
        SelectedCpuBenchmarkName = "Процесор";
        SelectedGpuBenchmarkName = "Відеокарта";
    }

    /// <summary>
    /// Updates the selected benchmark for a given category.
    /// </summary>
    /// <param name="category">The category of the benchmark (e.g., "CpuBenchmark", "GpuBenchmark").</param>
    /// <param name="id">The ID of the selected benchmark.</param>
    public async Task UpdateBenchmark(string category, Guid id)
    {
        var reqs = Requirements;
        if (category == "CpuBenchmark")
        {
            reqs.CpuBenchmark = id;
            var benchmark = await _requests.GetItem<CpuBenchmarkDtoModel>("/benchmarks/cpu/byId", id);
            SelectedCpuBenchmarkName = benchmark.Name;
        }
        else if (category == "GpuBenchmark")
        {
            reqs.GpuBenchmark = id;
            var benchmark = await _requests.GetItem<GpuBenchmarkDtoModel>("/benchmarks/gpu/byId", id);
            SelectedGpuBenchmarkName = benchmark.Name;
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
            storageNames.Add(drive.Name);
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
        reqs.RamInMegabytes = RamInGigabytes ?? 0;
        reqs.SpaceOnDiskInGigabytes = SpaceOnDiskInGigabytes ?? 0;
        reqs.SsdRequired = SsdRequired;
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
            ResultMessage = "Ваш ПК не зможе це запустити. Ось чому:\n" + string.Join("\n", messages.Select(kvp => kvp.Value));
        }
    }
}