using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PCFromScratch.Repository;
using PCFromScratch.Scrapers;

namespace PCFromScratch.App.ViewModels;

public class ComponentsCompareViewModel : INotifyPropertyChanged
{
    private readonly ICpuRepository _cpuRepository;
    private readonly IGpuRepository _gpuRepository;

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
            CompareComponents();
        }
    }
    
    public string? SelectedComponent2
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged();
            CompareComponents();
        }
    }

    public ObservableCollection<BenchmarkChartEntry> ChartData { get; } = new();

    public ComponentsCompareViewModel(ICpuRepository cpuRepository, IGpuRepository gpuRepository)
    {
        _cpuRepository = cpuRepository;
        _gpuRepository = gpuRepository;
    }

    private async void LoadComponents()
    {
        ComponentNames1.Clear();
        ComponentNames2.Clear();

        if (SelectedComponentType == "CPU")
        {
            await foreach (var cpu in _cpuRepository.GetCpus())
            {
                ComponentNames1.Add(cpu.Name);
                ComponentNames2.Add(cpu.Name);
            }
        }
        else if (SelectedComponentType == "GPU")
        {
            await foreach (var gpu in _gpuRepository.GetGpus())
            {
                ComponentNames1.Add(gpu.Name);
                ComponentNames2.Add(gpu.Name);
            }
        }
    }

    private void CompareComponents()
    {
        if (string.IsNullOrEmpty(SelectedComponent1) || string.IsNullOrEmpty(SelectedComponent2))
        {
            return;
        }

        ChartData.Clear();
        var screenWidth = DeviceDisplay.MainDisplayInfo.Width;
        if (SelectedComponentType == "CPU")
        {
            var benchmark1 = CpuMatcher.GetBenchmark(SelectedComponent1);
            var benchmark2 = CpuMatcher.GetBenchmark(SelectedComponent2);
            var score1 = int.Parse(benchmark1.BenchmarkScore.Replace(",", ""));
            var score2 = int.Parse(benchmark2.BenchmarkScore.Replace(",", ""));
            var width1 = (score1 * screenWidth / int.Max(score1, score2)) * 0.6;
            var width2 = (score2 * screenWidth / int.Max(score1, score2)) * 0.6;
            ChartData.Add(new BenchmarkChartEntry(SelectedComponent1, score1, width1));
            ChartData.Add(new BenchmarkChartEntry(SelectedComponent2, score2, width2));
        }
        else if (SelectedComponentType == "GPU")
        {
            var benchmark1 = GpuMatcher.GetBenchmark(SelectedComponent1);
            var benchmark2 = GpuMatcher.GetBenchmark(SelectedComponent2);
            var score1 = int.Parse(benchmark1.BenchmarkScore.Replace(",", ""));
            var score2 = int.Parse(benchmark2.BenchmarkScore.Replace(",", ""));
            var width1 = (score1 * screenWidth / int.Max(score1, score2)) * 0.6;
            var width2 = (score2 * screenWidth / int.Max(score1, score2)) * 0.6;
            ChartData.Add(new BenchmarkChartEntry(SelectedComponent1, score1, width1));
            ChartData.Add(new BenchmarkChartEntry(SelectedComponent2, score2, width2));
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