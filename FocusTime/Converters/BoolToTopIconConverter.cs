using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FocusTime.Converters;

public class BoolToTopIconConverter : IValueConverter
{
    public static readonly BoolToTopIconConverter Instance = new();

    // 置顶图标 (Pin Icon - Filled)
    private const string PinnedIcon = "M16,12V4H17V2H7V4H8V12L6,14V16H11.2V22H12.8V16H18V14L16,12Z";
    
    // 未置顶图标 (Pin Icon - Outline)  
    private const string UnpinnedIcon = "M14,4V12L16,14V16H13V22H11V16H8V14L10,12V4H14M16,2H8V4H10V11.2L8,13.2V14H8.5V2H16V2Z";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isTopmost)
        {
            return isTopmost ? PinnedIcon : UnpinnedIcon;
        }
        return UnpinnedIcon;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}