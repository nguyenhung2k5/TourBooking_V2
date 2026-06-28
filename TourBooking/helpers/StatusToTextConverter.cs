using System;
using System.Globalization;
using System.Windows.Data;
using TourBooking.Models;

namespace TourBooking.Helpers
{
    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BookingStatus status)
            {
                switch (status)
                {
                    case BookingStatus.Paid:
                        return "Đã thanh toán";
                    case BookingStatus.Cancelled:
                        return "Đã hủy";
                    case BookingStatus.Refunded:
                        return "Đã hoàn tiền";
                }
            }
            return value?.ToString() ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
