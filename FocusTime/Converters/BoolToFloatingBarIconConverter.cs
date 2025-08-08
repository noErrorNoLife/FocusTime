using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FocusTime.Converters;

public class BoolToFloatingBarIconConverter : IValueConverter
{
    public static readonly BoolToFloatingBarIconConverter Instance = new();

    // 悬浮条激活图标 (窗口最小化)
    private const string ActiveIcon = "M4,14H20V16H4V14Z";
    
    // 悬浮条未激活图标 (窗口图标)  
    private const string InactiveIcon = "M5,3C3.89,3 3,3.89 3,5V19C3,20.11 3.89,21 5,21H19C20.11,21 21,20.11 21,19V5C21,3.89 20.11,3 19,3H5M5,5H19V19H5V5Z";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isFloatingBarMode)
        {
            return isFloatingBarMode ? ActiveIcon : InactiveIcon;
        }
        return InactiveIcon;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}