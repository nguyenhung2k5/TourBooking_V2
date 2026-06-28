using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using TourBooking.Services;

namespace TourBooking.Views.Dialogs
{
    public partial class ReportExportDialog : Window
    {
        private readonly ReportService _reportService;

        public ReportExportDialog()
        {
            InitializeComponent();
            _reportService = new ReportService();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            string reportType = "Tour";
            string defaultFileName = "BaoCao_QuanLyTour";
            if (rbBooking.IsChecked == true)
            {
                reportType = "Booking";
                defaultFileName = "BaoCao_QuanLyDonHang";
            }
            else if (rbStaff.IsChecked == true)
            {
                reportType = "Staff";
                defaultFileName = "BaoCao_QuanLyNhanSu";
            }
            else if (rbCustomer.IsChecked == true)
            {
                reportType = "Customer";
                defaultFileName = "BaoCao_QuanLyKhachHang";
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Tập tin CSV (*.csv)|*.csv|Tất cả tập tin (*.*)|*.*",
                FileName = $"{defaultFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                Title = "Chọn nơi lưu file báo cáo"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string targetFilePath = saveFileDialog.FileName;
                if (_reportService.ExportReportToFile(reportType, targetFilePath))
                {
                    MessageBoxResult result = MessageBox.Show($"Xuất báo cáo thành công!\nFile đã được lưu tại:\n{targetFilePath}\n\nBạn có muốn mở file ngay không?", "Thành công", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(targetFilePath) { UseShellExecute = true });
                    }
                    Close();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi xuất báo cáo!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
