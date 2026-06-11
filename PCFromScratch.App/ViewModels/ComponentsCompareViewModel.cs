using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

public class ComponentsCompareViewModel : INotifyPropertyChanged
{
    private readonly ServerRequests _requests;

    public ObservableCollection<string> ComponentTypes { get; } = new() { "CPU", "GPU" };
    public string? SelectedComponentType
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged();
            LoadComponents();
        }
    }

    public ObservableCollection<string> ComponentNames1 { get; } = new();
    public ObservableCollection<string> ComponentNames2 { get; } = new();
    
    public string? SelectedComponent1
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged();
            _ = CompareComponents();
        }
    }
    
    public string? SelectedComponent2
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged();
            _ = CompareComponents();
        }
    }

    public ObservableCollection<BenchmarkChartEntry> ChartData { get; } = new();

    public ComponentsCompareViewModel(ServerRequests requests)
    {
        _requests = requests;
    }

    private async void LoadComponents()
    {
        ComponentNames1.Clear();
        ComponentNames2.Clear();

        if (SelectedComponentType == "CPU")
        {
            foreach (var cpu in await _requests.GetItems<CpuDtoModel>("/cpu") ?? [])
            {
                ComponentNames1.Add(cpu.Name);
                ComponentNames2.Add(cpu.Name);
            }
        }
        else if (SelectedComponentType == "GPU")
        {
            foreach (var gpu in await _requests.GetItems<GpuDtoModel>("/gpu") ?? [])
            {
                ComponentNames1.Add(gpu.Name);
                ComponentNames2.Add(gpu.Name);
            }
        }
    }

    private async Task CompareComponents()
    {
        if (string.IsNullOrEmpty(SelectedComponent1) || string.IsNullOrEmpty(SelectedComponent2))
        {
            return;
        }

        ChartData.Clear();
        var screenWidth = DeviceDisplay.MainDisplayInfo.Width;
        if (SelectedComponentType == "CPU")
        {
            var benchmark1 = await _requests.GetCpuBenchmark(SelectedComponent1);
            var benchmark2 = await _requests.GetCpuBenchmark(SelectedComponent2);
            if (benchmark1 is null || benchmark2 is null) return;
            var width1 = (benchmark1.Value.Score * screenWidth / int.Max(benchmark1.Value.Score, benchmark2.Value.Score)) * 0.6;
            var width2 = (benchmark2.Value.Score * screenWidth / int.Max(benchmark1.Value.Score, benchmark2.Value.Score)) * 0.6;
            ChartData.Add(new BenchmarkChartEntry(SelectedComponent1, benchmark1.Value.Score, width1));
            ChartData.Add(new BenchmarkChartEntry(SelectedComponent2, benchmark2.Value.Score, width2));
        }
        else if (SelectedComponentType == "GPU")
        {
            var benchmark1 = await _requests.GetGpuBenchmark(SelectedComponent1);
            var benchmark2 = await _requests.GetGpuBenchmark(SelectedComponent2);
            if (benchmark1 is null || benchmark2 is null) return;
            var width1 = (benchmark1.Value.Score * screenWidth / int.Max(benchmark1.Value.Score, benchmark2.Value.Score)) * 0.6;
            var width2 = (benchmark2.Value.Score * screenWidth / int.Max(benchmark1.Value.Score, benchmark2.Value.Score)) * 0.6;
            ChartData.Add(new BenchmarkChartEntry(SelectedComponent1, benchmark1.Value.Score, width1));
            ChartData.Add(new BenchmarkChartEntry(SelectedComponent2, benchmark2.Value.Score, width2));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class BenchmarkChartEntry (string name, int value, double width)
{
    public string Name { get; set; } = name;
    public int Value { get; set; } = value;
    public double Width { get; set; } = width;
}