using System.Windows;
using System.Windows.Controls;
using TourBooking.ViewModels;

namespace TourBooking.Views
{
    public partial class TourSearchView : UserControl
    {
        public TourSearchView()
        {
            InitializeComponent();
            DataContext = new TourListViewModel();
            ((TourListViewModel)DataContext).Load();
        }

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
            var viewModel = DataContext as TourListViewModel;
            viewModel?.Search();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as TourListViewModel;
            viewModel?.Search();
        }
    }
}
