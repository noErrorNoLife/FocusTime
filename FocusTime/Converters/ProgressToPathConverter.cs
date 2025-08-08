using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace FocusTime.Converters;

public class ProgressToPathConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double progress)
        {
            var geometry = new PathGeometry();
            var figure = new PathFigure { IsClosed = false };

            double angle = (progress / 100.0) * 360;
            if (angle >= 360)
            {
                angle = 359.999;
            }

            double radius = 132; // (280 / 2) - (8 * 2) = 140 - 8 = 132
            var center = new Avalonia.Point(140, 140);
            var startPoint = new Avalonia.Point(140, 8); // center.X, center.Y - radius

            figure.StartPoint = startPoint;

            double angleRad = (Math.PI / 180.0) * (angle - 90);
            double x = center.X + radius * Math.Cos(angleRad);
            double y = center.Y + radius * Math.Sin(angleRad);

            var endPoint = new Avalonia.Point(x, y);

            var arcSegment = new ArcSegment
            {
                Point = endPoint,
                Size = new Avalonia.Size(radius, radius),
                IsLargeArc = angle > 180,
                SweepDirection = SweepDirection.Clockwise
            };
            
            figure.Segments.Add(arcSegment);
            geometry.Figures.Add(figure);
            return geometry;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 