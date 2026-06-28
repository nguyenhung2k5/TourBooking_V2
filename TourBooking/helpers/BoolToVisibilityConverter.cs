using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TourBooking.Helpers
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool val)
            {
                flag = val;
            }

            // Check if we want to invert the boolean value
            bool invert = false;
            if (parameter != null)
            {
                string paramStr = parameter.ToString().ToLower();
                if (paramStr == "inverse" || paramStr == "true" || paramStr == "invert")
                {
                    invert = true;
                }
            }

            if (invert)
            {
                flag = !flag;
            }

            return flag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool flag = visibility == Visibility.Visible;
                
                bool invert = false;
                if (parameter != null)
                {
                    string paramStr = parameter.ToString().ToLower();
                    if (paramStr == "inverse" || paramStr == "true" || paramStr == "invert")
                    {
                        invert = true;
                    }
                }

                if (invert)
                {
                    flag = !flag;
                }

                return flag;
            }

            return false;
        }
    }
}
