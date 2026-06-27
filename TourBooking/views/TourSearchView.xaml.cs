using System.Windows;
using System.Windows.Controls;

namespace TourBooking.Views
{
    /// <summary>
    /// Interaction logic for TourSearchView.xaml
    /// </summary>
    public partial class TourSearchView : UserControl
    {
        public TourSearchView()
        {
            InitializeComponent();
        }

        // ========================================================
        // LOGIC ĐIỀU HƯỚNG CHUYỂN TRANG
        // ========================================================

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new DashboardView());
        }

        private void BtnBookingCreate_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new BookingCreateView());
        }

        private void BtnOrderManagement_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new OrderManagementView());
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng lọc kết quả đang được xây dựng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}