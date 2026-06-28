using System.Windows;
using System.Windows.Controls;
using TourBooking.ViewModels;

namespace TourBooking.Views
{
    public partial class BookingView : UserControl
    {
        public BookingView()
        {
            InitializeComponent();
            DataContext = new BookingViewModel();
            ((BookingViewModel)DataContext).Load();
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

        private void BtnOrderManagement_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new MyBookingsView());
        }
    }
}
