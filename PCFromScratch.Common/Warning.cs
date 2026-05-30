using CommunityToolkit.Mvvm.ComponentModel;

namespace PCFromScratch.Common;
public partial class Warning : ObservableObject
{
    [ObservableProperty]
    private EnumWithName<WarningSeverity> _severity;
    [ObservableProperty]
    private string _message;

    public Warning(WarningSeverity severity, string message)
    {
        _severity = severity.GetEnumWithName();
        _message = message;
    }
    
}