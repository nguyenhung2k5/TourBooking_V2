using System.Globalization;
using System.Windows;
using TourBooking.Models;
using TourBooking.Services;

namespace TourBooking.Views.Dialogs
{
    public partial class TourDetailDialog : Window
    {
        public bool BookNowClicked { get; private set; }

        public TourDetailDialog(Tour tour)
        {
            InitializeComponent();
            if (tour != null)
            {
                txtTourCode.Text = tour.TourCode;
                txtTourName.Text = tour.TourName;
                txtDestination.Text = tour.Destination;
                txtDepartureDate.Text = tour.DepartureDate.ToString("dd/MM/yyyy");
                txtReturnDate.Text = tour.ReturnDate.ToString("dd/MM/yyyy");
                txtTotalSlots.Text = $"{tour.TotalSlots} chỗ";
                txtAvailableSlots.Text = $"{tour.AvailableSlots} chỗ";
                txtPriceAdult.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", tour.PriceAdult);
                txtPriceChild.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} đ", tour.PriceChild);

                if (tour.IsActive)
                {
                    txtStatus.Text = "● Đang hoạt động";
                    txtStatus.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    txtStatus.Text = "● Tạm dừng hoạt động";
                    txtStatus.Foreground = System.Windows.Media.Brushes.Red;
                }
            }

            if (SessionService.IsAdmin)
            {
                btnBookNow.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnBookNow_Click(object sender, RoutedEventArgs e)
        {
            BookNowClicked = true;
            DialogResult = true;
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
