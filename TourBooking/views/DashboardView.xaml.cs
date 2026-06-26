using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TourBooking.Data; // Thư mục chứa AppDbContext của bạn

namespace TourBooking.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();

            // Kích hoạt nạp dữ liệu thật từ database SQL Server
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // 1. TRUY VẤN THÔNG TIN NHÂN VIÊN ĐANG ĐĂNG NHẬP
                    var currentStaff = db.Staffs.FirstOrDefault(s => s.Username == "admin");
                    if (currentStaff != null)
                    {
                        txtHeaderStaffName.Text = $"Chào buổi sáng, {currentStaff.FullName}";
                        txtProfileStaffName.Text = currentStaff.FullName;
                    }

                    // 2. TRUY VẤN DỮ LIỆU BẢNG TOURS ĐỂ TÍNH KPI SỐ LIỆU THẬT
                    var realTours = db.Tours.ToList();

                    // Đếm tổng số Tour
                    txtTotalTickets.Text = realTours.Count.ToString();

                    // ĐÃ SỬA LỖI ÉP KIỂU SANG DECIMAL TẠI ĐÂY ĐỂ ĐỒNG BỘ VỚI CƠ SỞ DỮ LIỆU
                    decimal totalRevenueMock = realTours.Sum(t => (decimal)t.PriceAdult * (t.TotalSlots - t.AvailableSlots));
                    txtTodayRevenue.Text = string.Format("{0:N0} đ", totalRevenueMock);

                    // 3. ĐỔ DỮ LIỆU THẬT VÀO BẢNG DATAGRID TRANSACTION
                    var transactionList = realTours.Select(t => new OrderSample
                    {
                        Id = $"#TR-{t.TourCode}",
                        Customer = "Hệ thống tự động",
                        Service = t.TourName,
                        Status = t.IsActive ? "HOẠT ĐỘNG" : "TẠM DỪNG",
                        StatusBg = t.IsActive ? "#E6F4EA" : "#FCE8E6",
                        StatusFg = t.IsActive ? "#137333" : "#C5221F",
                        Price = string.Format("{0:N0} đ", t.PriceAdult)
                    }).ToList();

                    // Nạp nguồn dữ liệu chuẩn vào DataGrid
                    dgRecentTransactions.ItemsSource = transactionList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi tải dữ liệu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class OrderSample
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public string Service { get; set; }
        public string Status { get; set; }
        public string StatusBg { get; set; }
        public string StatusFg { get; set; }
        public string Price { get; set; }
    }
}