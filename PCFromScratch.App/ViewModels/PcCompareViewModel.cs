using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PCFromScratch.DTOModels;
using PCFromScratch.App.Utils;
using PCFromScratch.Common;

namespace PCFromScratch.App.ViewModels;

public class PcCompareViewModel : INotifyPropertyChanged
{
    private readonly ServerRequests _serverRequests;

    private PcModel Pc1 { get; set; } = new();
    private PcModel Pc2 { get; set; } = new();

    public string Pc1CpuName { get => field; set { field = value; OnPropertyChanged(); } } = "";
    public string Pc1GpuName { get => field; set { field = value; OnPropertyChanged(); } } = "";
    public string Pc1RamName { get => field; set { field = value; OnPropertyChanged(); } } = "";
    public string Pc1PsuName { get => field; set { field = value; OnPropertyChanged(); } } = "";
    public ObservableCollection<string> Pc1StorageNames { get; set; } = new();

    public string Pc2CpuName { get => field; set { field = value; OnPropertyChanged(); } } = "";
    public string Pc2GpuName { get => field; set { field = value; OnPropertyChanged(); } } = "";
    public string Pc2RamName { get => field; set { field = value; OnPropertyChanged(); } } = "";
    public string Pc2PsuName { get => field; set { field = value; OnPropertyChanged(); } } = "";
    public ObservableCollection<string> Pc2StorageNames { get; set; } = new();

    public Color Pc1CpuColor { get => field; set { field = value; OnPropertyChanged(); } } = Color.FromArgb("#212121");
    public Color Pc1GpuColor { get => field; set { field = value; OnPropertyChanged(); } } = Color.FromArgb("#212121");
    public Color Pc1RamColor { get => field; set { field = value; OnPropertyChanged(); } } = Color.FromArgb("#212121");
    public Color Pc1PsuColor { get => field; set { field = value; OnPropertyChanged(); } } = Color.FromArgb("#212121");
    public Color Pc1StorageColor { get => field; set { field = value; OnPropertyChanged(); } } = Color.FromArgb("#212121");

    public Color Pc2CpuColor { get => field; set { field = value; OnPropertyChanged(); } } = Color.FromArgb("#212121");
    public Color Pc2GpuColor { get => field; set { field = value; OnPropertyChanged(); } } = Color.FromArgb("#212121");
    public Color Pc2RamColor { get => field; set { field = value; OnPropertyChanged(); } } = Color.FromArgb("#212121");
    public Color Pc2PsuColor { get => field; set { field = value; OnPropertyChanged(); } } = Color.FromArgb("#212121");
    public Color Pc2StorageColor { get => field; set { field = value; OnPropertyChanged(); } } = Color.FromArgb("#212121");

    public ICommand CompareCommand { get; }

    public PcCompareViewModel(ServerRequests serverRequests)
    {
        _serverRequests = serverRequests;
        CompareCommand = new AsyncRelayCommand(Compare);
    }

    private async Task Compare()
    {
        var results = await _serverRequests.ComparePcs(
            new PcDtoModel(Pc1.Cpu, Pc1.Motherboard, Pc1.Gpu, Pc1.Ram, Pc1.Cooler, Pc1.InternalDrives, Pc1.Psu), 
            new PcDtoModel(Pc2.Cpu, Pc2.Motherboard, Pc2.Gpu, Pc2.Ram, Pc2.Cooler, Pc2.InternalDrives, Pc2.Psu));
        if (results == null) return;

        foreach (var result in results)
        {
            var (pc1Color, pc2Color) = result.Metric switch
            {
                PcCompareMetric.Better => (Colors.Green, Colors.Red),
                PcCompareMetric.Worse => (Colors.Red, Colors.Green),
                _ => (Color.FromArgb("#212121"), Color.FromArgb("#212121"))
            };

            switch (result.Component)
            {
                case "Cpu":
                    Pc1CpuColor = pc1Color;
                    Pc2CpuColor = pc2Color;
                    break;
                case "Gpu":
                    Pc1GpuColor = pc1Color;
                    Pc2GpuColor = pc2Color;
                    break;
                case "Ram":
                    Pc1RamColor = pc1Color;
                    Pc2RamColor = pc2Color;
                    break;
                case "Psu":
                    Pc1PsuColor = pc1Color;
                    Pc2PsuColor = pc2Color;
                    break;
                case "Storage":
                    Pc1StorageColor = pc1Color;
                    Pc2StorageColor = pc2Color;
                    break;
            }
        }
    }

    public async Task UpdateComponent(string category, Guid selectedPartId, int pcIndex)
    {
        string name;
        try 
        {
            name = category switch
            {
                "Cpu" => (await _serverRequests.GetItem<CpuDtoModel>("/cpu", selectedPartId)).Name,
                "Psu" => (await _serverRequests.GetItem<PsuDtoModel>("/psu", selectedPartId)).Name,
                "Ram" => (await _serverRequests.GetItem<RamDtoModel>("/ram", selectedPartId)).Model,
                "Storage" => (await _serverRequests.GetItem<InternalDriveDtoModel>("/drive", selectedPartId)).Name,
                "Gpu" => (await _serverRequests.GetItem<GpuDtoModel>("/gpu", selectedPartId)).Name,
                _ => string.Empty
            };
        }
        catch { return; }

        if (string.IsNullOrEmpty(name)) return;

        var pc = pcIndex == 1 ? Pc1 : Pc2;

        switch (category)
        {
            case "Cpu":
                pc.Cpu = selectedPartId;
                if (pcIndex == 1) Pc1CpuName = name; else Pc2CpuName = name;
                break;
            case "Psu":
                pc.Psu = selectedPartId;
                if (pcIndex == 1) Pc1PsuName = name; else Pc2PsuName = name;
                break;
            case "Ram":
                pc.Ram = selectedPartId;
                if (pcIndex == 1) Pc1RamName = name; else Pc2RamName = name;
                break;
            case "Storage":
                pc.InternalDrives.Add(selectedPartId);
                if (pcIndex == 1) Pc1StorageNames.Add(name); else Pc2StorageNames.Add(name);
                break;
            case "Gpu":
                pc.Gpu = selectedPartId;
                if (pcIndex == 1) Pc1GpuName = name; else Pc2GpuName = name;
                break;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

class PcModel
{
    public Guid? Cpu;
    public Guid? Motherboard;
    public Guid? Gpu;
    public Guid? Ram;
    public Guid? Cooler;
    public List<Guid> InternalDrives = [];
    public Guid? Psu;
}