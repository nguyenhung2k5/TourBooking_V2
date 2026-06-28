using System.Windows;
using System.Windows.Controls;
using TourBooking.Services;
using TourBooking.ViewModels;

namespace TourBooking.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
            ((DashboardViewModel)DataContext).Load();
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

        private void BtnOrderManagement_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new MyBookingsView());
        }

        private void BtnReportExport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Views.Dialogs.ReportExportDialog();
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void BtnBell_Click(object sender, RoutedEventArgs e)
        {
            popNotifications.IsOpen = !popNotifications.IsOpen;
        }

        private void CloseNotifications_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            popNotifications.IsOpen = false;
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
