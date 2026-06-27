using System.Windows;
using System.Windows.Controls;
using TourBooking.Views; // Gọi namespace chứa các View con của bạn

namespace TourBooking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Vừa mở ứng dụng lên, nạp sẵn màn hình Bảng điều khiển (Dashboard) vào khung chờ
            SwitchView(new DashboardView());
        }

        /// <summary>
        /// Hàm dùng chung để bất kỳ trang con nào cũng có thể ra lệnh đổi màn hình bên phải
        /// </summary>
        public void SwitchView(UserControl newView)
        {
            MainContentContainer.Content = newView;
        }
    }
}