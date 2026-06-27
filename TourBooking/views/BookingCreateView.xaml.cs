using System.Windows;
using System.Windows.Controls;

namespace TourBooking.Views
{
    /// <summary>
    /// Interaction logic for BookingCreateView.xaml
    /// </summary>
    public partial class BookingCreateView : UserControl
    {
        public BookingCreateView()
        {
            InitializeComponent();
        }

        // ========================================================
        // LOGIC ĐIỀU HƯỚNG SIDEBAR
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

        private void BtnOrderManagement_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new OrderManagementView());
        }
    }
}