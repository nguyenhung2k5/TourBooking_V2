using System.Windows.Controls;

namespace TourBooking.Views
{
    /// <summary>
    /// Interaction logic for OrderManagementView.xaml
    /// </summary>
    public partial class OrderManagementView : UserControl
    {
        public OrderManagementView()
        {
            InitializeComponent();

            // Hiện tại Database trống trơn nên không cần gán ItemsSource.
            // Bảng DataGrid và các ô thống kê sẽ trống dữ liệu chuẩn chỉnh.
        }
    }
}