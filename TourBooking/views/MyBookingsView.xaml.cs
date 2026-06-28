using System.Windows;
using System.Windows.Controls;
using TourBooking.Services;
using TourBooking.ViewModels;

namespace TourBooking.Views
{
    public partial class MyBookingsView : UserControl
    {
        public MyBookingsView()
        {
            InitializeComponent();
            DataContext = new MyBookingsViewModel();
            ((MyBookingsViewModel)DataContext).Load();
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new DashboardView());
        }

        private void BtnTourSearch_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new TourListView());
        }

        private void BtnBookingCreate_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new BookingView());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SessionService.Logout();
                var mainWindow = Window.GetWindow(this) as MainWindow;
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                mainWindow?.Close();
            }
        }
    }
}
