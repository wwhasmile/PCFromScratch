using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PCFromScratch.App.Pages
{
    public class TypeCheckConverter : IValueConverter
    {
        public Type TargetType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType() == TargetType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
