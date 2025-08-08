using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FocusTime.Converters;

public class BoolToFloatingBarTooltipConverter : IValueConverter
{
    public static readonly BoolToFloatingBarTooltipConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isFloatingBarMode)
        {
            return isFloatingBarMode ? "关闭悬浮条" : "开启悬浮条";
        }
        return "开启悬浮条";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}