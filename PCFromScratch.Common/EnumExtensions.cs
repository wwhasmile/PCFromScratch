using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PCFromScratch.Common;

public sealed record EnumWithName<TEnum>(TEnum Value, string DisplayName) where TEnum : struct, Enum;
public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name is null)
            return value.ToString();

        var field = type.GetField(name);
        var display = field?.GetCustomAttribute<DisplayAttribute>();
        return display?.Name ?? name;
    }

    public static EnumWithName<TEnum> GetEnumWithName<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        return new EnumWithName<TEnum>(value, value.GetDisplayName());
    }

    public static EnumWithName<TEnum>[] GetValueWithNames<TEnum>() where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>();
        var result = new EnumWithName<TEnum>[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            result[i] = values[i].GetEnumWithName();
        }
        return result;
    }
}