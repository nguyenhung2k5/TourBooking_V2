using System.Windows;
using TourBooking.Views;    
using TourBooking.Services;

namespace TourBooking
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SwitchView(new DashboardView());
        }

        public void SwitchView(object newView)
        {
            MainContentContainer.Content = newView;
        }
    }
}