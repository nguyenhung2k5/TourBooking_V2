using System;
using System.Globalization;
using System.Windows.Data;

namespace TourBooking.Helpers
{
    public class NumberFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "0 đ";

            if (decimal.TryParse(value.ToString(), out decimal numericValue))
            {
                return string.Format(new CultureInfo("vi-VN"), "{0:N0} đ", numericValue);
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0m;

            string cleanValue = value.ToString()
                .Replace("đ", "")
                .Replace(".", "")
                .Replace(",", "")
                .Trim();

            if (decimal.TryParse(cleanValue, out decimal numericValue))
            {
                return numericValue;
            }

            return 0m;
        }
    }
}
