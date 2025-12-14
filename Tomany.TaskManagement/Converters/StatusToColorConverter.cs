using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Tomany.TaskManagement.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "in progress" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f59e0b")),
                    "completed" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10b981")),
                    "pending" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6b7280")),
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#52725b"))
                };
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#52725b"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
