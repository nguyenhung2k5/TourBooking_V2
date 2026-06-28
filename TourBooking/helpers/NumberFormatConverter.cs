using System;
using System.Globalization;
using System.Windows.Data;

namespace TourBooking.Helpers
{
    public class NumberFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal || value is double || value is int || value is long)
            {
                return string.Format(new CultureInfo("vi-VN"), "{0:C0}", value);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}