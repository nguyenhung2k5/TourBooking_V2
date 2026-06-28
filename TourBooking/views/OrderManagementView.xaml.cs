using System.Windows;
using System.Windows.Controls;
using TourBooking.ViewModels;

namespace TourBooking.Views
{
    public partial class OrderManagementView : UserControl
    {
        public OrderManagementView()
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
            mainWindow?.SwitchView(new TourSearchView());
        }

        private void BtnBookingCreate_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new BookingCreateView());
        }
    }
}
