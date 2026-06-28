using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TourBooking.Models;

namespace TourBooking.Helpers
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BookingStatus status)
            {
                switch (status)
                {
                    case BookingStatus.Pending:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F79009")); // Vàng cam
                    case BookingStatus.Paid:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#12B76A")); // Xanh lá
                    case BookingStatus.Cancelled:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F04438")); // Đỏ
                    case BookingStatus.Refunded:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E90FA")); // Xanh dương
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
