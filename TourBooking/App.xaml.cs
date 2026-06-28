using System;
using System.Linq;
using System.Windows;
using TourBooking.Data; // Khai báo đường dẫn đến AppDbContext
using TourBooking.ViewModels;
using TourBooking.Views.Admin;

namespace TourBooking
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ====================================================
            // 1. TEST KẾT NỐI DATABASE THẬT TRƯỚC KHI LÊN GIAO DIỆN
            // ====================================================
            bool isDbConnected = false;
            try
            {
                using (var context = new AppDbContext())
                {
                    // Thử truy vấn 1 bản ghi bất kỳ để ép Entity Framework kết nối vào SQL Server
                    var testConnection = context.Staffs.FirstOrDefault();
                    isDbConnected = true;
                }

                // Nếu chạy qua dòng trên mà không lỗi, chứng tỏ SQL Server đã thông!
                MessageBox.Show("Tuyệt vời! Ứng dụng đã kết nối THÀNH CÔNG với cơ sở dữ liệu thật.\n\nNhấn OK để tải giao diện Dashboard.",
                                "Thông báo Kết nối", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Nếu báo lỗi đỏ ở đây, 100% là do cấu hình chuỗi kết nối (ConnectionString) trong App.config hoặc SQL Server chưa bật
                MessageBox.Show($"CẢNH BÁO: KHÔNG THỂ KẾT NỐI TỚI DATABASE!\n\nLỗi chi tiết: {ex.Message}\n\nHệ thống vẫn sẽ mở giao diện nhưng dữ liệu (DataGrid) sẽ trống.",
                                "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // ====================================================
            // 2. KHỞI TẠO VÀ HIỂN THỊ GIAO DIỆN DASHBOARD TỔNG
            // ====================================================
            var dashboardVM = new AdminDashboardViewModel();
            var dashboardView = new AdminDashboardView();

            // Gán ViewModel vào View để các nút bấm trên Sidebar nhận lệnh
            dashboardView.DataContext = dashboardVM;

            // Mượn 1 cái Window trống để bọc UserControl Dashboard lại (Dùng để test)
            Window testWindow = new Window
            {
                Title = isDbConnected ? "Hệ thống Quản lý Tour (ĐÃ KẾT NỐI DB)" : "Hệ thống Quản lý Tour (MẤT KẾT NỐI DB)",
                Width = 1400,
                Height = 900,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = dashboardView
            };

            testWindow.Show();
        }
    }
}