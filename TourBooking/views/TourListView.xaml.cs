using System.Windows;
using System.Windows.Controls;
using TourBooking.Models;
using TourBooking.ViewModels;
using TourBooking.Views.Dialogs;

namespace TourBooking.Views
{
    public partial class TourListView : UserControl
    {
        public TourListView()
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
            mainWindow?.SwitchView(new BookingView());
        }

        private void BtnOrderManagement_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchView(new MyBookingsView());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as TourListViewModel;
            viewModel?.Search();
        }

        private void BtnDetail_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tour = button?.DataContext as Tour;
            if (tour != null)
            {
                var dialog = new TourDetailDialog(tour)
                {
                    Owner = Window.GetWindow(this)
                };
                
                if (dialog.ShowDialog() == true && dialog.BookNowClicked)
                {
                    var mainWindow = Window.GetWindow(this) as MainWindow;
                    var bookingView = new BookingView();
                    var bookingVm = bookingView.DataContext as BookingViewModel;
                    if (bookingVm != null)
                    {
                        // Chọn Tour vừa kích hoạt đặt vé
                        bookingVm.SelectedTour = tour;
                    }
                    mainWindow?.SwitchView(bookingView);
                }
            }
        }
    }
}
