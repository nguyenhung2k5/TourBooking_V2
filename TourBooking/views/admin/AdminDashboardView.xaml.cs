using System.Windows.Controls;

namespace TourBooking.Views.Admin
{
    
    public partial class AdminDashboardView : UserControl
    {
        public AdminDashboardView()
        {
            InitializeComponent();
        }

        private void BtnBell_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            popNotifications.IsOpen = !popNotifications.IsOpen;
        }

        private void CloseNotifications_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            popNotifications.IsOpen = false;
        }
    }
}