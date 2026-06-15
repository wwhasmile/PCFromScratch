using System.Globalization;

namespace PCFromScratch.App.Pages;

public class TypeCheckConverter : IValueConverter
{
    public Type TargetType { get; set; } = null!;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.GetType() == TargetType;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}