using System.Windows;
using System.Windows.Controls;

namespace TourBooking.Views
{
    /// <summary>
    /// Interaction logic for OrderManagementView.xaml
    /// </summary>
    public partial class OrderManagementView : UserControl
    {
        public OrderManagementView()
        {
            InitializeComponent();
        }

        // ========================================================
        // LOGIC ĐIỀU HƯỚNG SIDEBAR CHO TRANG QUẢN LÝ ĐƠN HÀNG
        // ========================================================

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new DashboardView());
        }

        private void BtnTourSearch_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new TourSearchView());
        }

        private void BtnBookingCreate_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new BookingCreateView());
        }
    }
}