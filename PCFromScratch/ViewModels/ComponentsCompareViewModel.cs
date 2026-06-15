using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

[QueryProperty(nameof(SelectedPartId), "SelectedPartId")]
[QueryProperty(nameof(Category), "Category")]
[QueryProperty(nameof(PcIndex), "PcIndex")]
public class ComponentsCompareViewModel : INotifyPropertyChanged
{
    private readonly ServerRequests _requests;
    private static readonly Color ActiveCategoryColor = Color.FromArgb("#0275c9");
    private static readonly Color InactiveCategoryColor = Application.Current?.RequestedTheme == AppTheme.Light? Color.FromArgb("#C8C8C8") : Color.FromArgb("#212121") ?? Color.FromArgb("#212121") ;

    public string SelectedComponentType { get; set; } = "CPU";

    public Guid SelectedPartId { get; set; }
    public string Category { get; set; }
    public int PcIndex { get; set; }

    public string Component1Name { get; set; } = "Оберіть компонент";
    public string Component2Name { get; set; } = "Оберіть компонент";

    public Color CpuButtonColor { get; set; } = ActiveCategoryColor;
    public Color GpuButtonColor { get; set; } = InactiveCategoryColor;

    public ObservableCollection<BenchmarkChartEntry> ChartData { get; } = new();
    public bool AreResultsAvailable => ChartData.Count > 0;
    
    public ICommand SelectComponentCommand { get; }
    public ICommand SetComponentTypeCommand { get; }

    public ComponentsCompareViewModel(ServerRequests requests)
    {
        _requests = requests;
        SelectComponentCommand = new AsyncRelayCommand<string>(SelectComponent);
        SetComponentTypeCommand = new Command<string>(SetComponentType);
    }

    private void SetComponentType(string componentType)
    {
        SelectedComponentType = componentType;
        Component1Name = "Оберіть компонент";
        Component2Name = "Оберіть компонент";
        ChartData.Clear();
        OnPropertyChanged(nameof(Component1Name));
        OnPropertyChanged(nameof(Component2Name));

        if (componentType == "CPU")
        {
            CpuButtonColor = ActiveCategoryColor;
            GpuButtonColor = InactiveCategoryColor;
        }
        else
        {
            CpuButtonColor = InactiveCategoryColor;
            GpuButtonColor = ActiveCategoryColor;
        }
        OnPropertyChanged(nameof(CpuButtonColor));
        OnPropertyChanged(nameof(GpuButtonColor));
    }
    
    private async Task SelectComponent(string? pcIndex)
    {
        var pageName = SelectedComponentType == "CPU" ? "CpuBenchmarkSelectionPage" : "GpuBenchmarkSelectionPage";
        await Shell.Current.GoToAsync($"{pageName}?PcIndex={pcIndex}");
    }

    public async Task UpdateSelectedComponent()
    {
        if (SelectedPartId == Guid.Empty) return;

        var benchmarkName = "";
        if (Category == "CpuBenchmark")
        {
            var benchmark = await _requests.GetItem<CpuBenchmarkDtoModel>("/benchmarks/cpu/byId", SelectedPartId);
            benchmarkName = benchmark.Name;
        }
        else if (Category == "GpuBenchmark")
        {
            var benchmark = await _requests.GetItem<GpuBenchmarkDtoModel>("/benchmarks/gpu/byId", SelectedPartId);
            benchmarkName = benchmark.Name;
        }

        if (PcIndex == 1)
        {
            Component1Name = benchmarkName;
        }
        else
        {
            Component2Name = benchmarkName;
        }
        
        OnPropertyChanged(nameof(Component1Name));
        OnPropertyChanged(nameof(Component2Name));

        await CompareComponents();
        
        Category = null;
        PcIndex = 0;
    }

    private async Task CompareComponents()
    {
        if (Component1Name == "Оберіть компонент" || Component2Name == "Оберіть компонент")
        {
            return;
        }

        ChartData.Clear();
        var screenWidth = DeviceDisplay.MainDisplayInfo.Width;
        if (SelectedComponentType == "CPU")
        {
            var benchmark1 = await _requests.GetCpuBenchmark(Component1Name);
            var benchmark2 = await _requests.GetCpuBenchmark(Component2Name);
            if (benchmark1 is null || benchmark2 is null) return;
            var width1 = (benchmark1.Value.Score * screenWidth / int.Max(benchmark1.Value.Score, benchmark2.Value.Score)) * 0.6;
            var width2 = (benchmark2.Value.Score * screenWidth / int.Max(benchmark1.Value.Score, benchmark2.Value.Score)) * 0.6;
            ChartData.Add(new BenchmarkChartEntry(Component1Name, benchmark1.Value.Score, width1));
            ChartData.Add(new BenchmarkChartEntry(Component2Name, benchmark2.Value.Score, width2));
        }
        else if (SelectedComponentType == "GPU")
        {
            var benchmark1 = await _requests.GetGpuBenchmark(Component1Name);
            var benchmark2 = await _requests.GetGpuBenchmark(Component2Name);
            if (benchmark1 is null || benchmark2 is null) return;
            var width1 = (benchmark1.Value.Score * screenWidth / int.Max(benchmark1.Value.Score, benchmark2.Value.Score)) * 0.6;
            var width2 = (benchmark2.Value.Score * screenWidth / int.Max(benchmark1.Value.Score, benchmark2.Value.Score)) * 0.6;
            ChartData.Add(new BenchmarkChartEntry(Component1Name, benchmark1.Value.Score, width1));
            ChartData.Add(new BenchmarkChartEntry(Component2Name, benchmark2.Value.Score, width2));
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