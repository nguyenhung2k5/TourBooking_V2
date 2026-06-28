using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TourBooking.Helpers
{
    public class SlotToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int slots)
            {
                if (slots == 0)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F04438")); // Đỏ
                }
                else if (slots <= 5)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F79009")); // Cam
                }
                else
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#12B76A")); // Xanh lá
                }
            }

            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
