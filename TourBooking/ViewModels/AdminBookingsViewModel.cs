using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TourBooking.Data;
using TourBooking.Models;
using TourBooking.Services;

namespace TourBooking.ViewModels
{
    public class AdminBookingsViewModel : BaseViewModel
    {
        private ObservableCollection<Booking> _allBookings;
        private ObservableCollection<Booking> _filteredBookings;
        private string _searchQuery;
        private string _selectedStatus = "Tất cả";
        private string _selectedStaff = "Tất cả";
        private DateTime? _selectedDate;
        private int _totalBookingsCount;
        private decimal _totalRevenueSum;

        public int TotalBookingsCount { get => _totalBookingsCount; set { _totalBookingsCount = value; OnPropertyChanged(); } }
        public decimal TotalRevenueSum { get => _totalRevenueSum; set { _totalRevenueSum = value; OnPropertyChanged(); } }

        public ObservableCollection<Booking> FilteredBookings
        {
            get => _filteredBookings;
            set { _filteredBookings = value; OnPropertyChanged(); }
        }

        public string SearchQuery { get => _searchQuery; set { _searchQuery = value; OnPropertyChanged(); } }
        public string SelectedStatus { get => _selectedStatus; set { _selectedStatus = value; OnPropertyChanged(); } }
        public string SelectedStaff { get => _selectedStaff; set { _selectedStaff = value; OnPropertyChanged(); } }
        public DateTime? SelectedDate { get => _selectedDate; set { _selectedDate = value; OnPropertyChanged(); } }

        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string> { "Tất cả", "Đã thanh toán", "Đã hủy", "Đã hoàn tiền" };
        public ObservableCollection<string> StaffOptions { get; set; } = new ObservableCollection<string> { "Tất cả" };

        public ICommand FilterCommand { get; }
        public ICommand ExportCsvCommand { get; }

        public AdminBookingsViewModel()
        {
            FilterCommand = new RelayCommand<object>(ExecuteFilter);

            ExportCsvCommand = new RelayCommand<object>(obj => {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Tập tin CSV (*.csv)|*.csv|Tất cả tập tin (*.*)|*.*",
                    FileName = $"BaoCao_QuanLyDonHang_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    Title = "Chọn nơi lưu file báo cáo Đơn hàng"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var reportService = new ReportService();
                    if (reportService.ExportReportToFile("Booking", saveFileDialog.FileName))
                    {
                        var result = MessageBox.Show($"Xuất báo cáo Đơn hàng thành công!\nFile đã được lưu tại:\n{saveFileDialog.FileName}\n\nBạn có muốn mở file ngay không?", "Thành công", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (result == MessageBoxResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                        }
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra khi xuất báo cáo Đơn hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });

            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var bookingsList = context.Bookings
                        .Include(b => b.Customer)
                        .Include(b => b.Tour)
                        .Include(b => b.Staff)
                        .Where(b => b.Status != BookingStatus.Pending)
                        .ToList();

                    TotalBookingsCount = bookingsList.Count;
                    TotalRevenueSum = bookingsList.Where(b => b.Status == BookingStatus.Paid).Sum(b => b.TotalAmount);

                    // Lựa chọn nhân viên chỉ hiển thị nhân viên (Role == UserRole.Staff)
                    var staffNames = context.Staffs
                        .Where(s => s.Role == UserRole.Staff)
                        .Select(s => s.FullName)
                        .ToList();

                    StaffOptions = new ObservableCollection<string> { "Tất cả" };
                    foreach (var name in staffNames)
                    {
                        if (!string.IsNullOrWhiteSpace(name) && !StaffOptions.Contains(name))
                            StaffOptions.Add(name);
                    }

                    _allBookings = new ObservableCollection<Booking>(bookingsList);
                    FilteredBookings = new ObservableCollection<Booking>(bookingsList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối bảng Đơn hàng: {ex.Message}", "Lỗi Hệ Thống");
                _allBookings = new ObservableCollection<Booking>();
                FilteredBookings = new ObservableCollection<Booking>();
            }
        }

        private void ExecuteFilter(object obj)
        {
            if (_allBookings == null) return;

            var result = _allBookings.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                result = result.Where(b =>
                    (b.BookingCode != null && b.BookingCode.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (b.Customer != null && b.Customer.FullName != null && b.Customer.FullName.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            if (SelectedStatus != "Tất cả")
            {
                BookingStatus? targetStatus = null;
                if (SelectedStatus == "Đã thanh toán") targetStatus = BookingStatus.Paid;
                else if (SelectedStatus == "Đã hủy") targetStatus = BookingStatus.Cancelled;
                else if (SelectedStatus == "Đã hoàn tiền") targetStatus = BookingStatus.Refunded;

                if (targetStatus.HasValue)
                {
                    result = result.Where(b => b.Status == targetStatus.Value);
                }
            }

            if (SelectedStaff != "Tất cả")
            {
                result = result.Where(b => b.Staff != null && b.Staff.FullName == SelectedStaff);
            }

            if (SelectedDate.HasValue)
            {
                result = result.Where(b => b.BookingDate.Date == SelectedDate.Value.Date);
            }

            FilteredBookings = new ObservableCollection<Booking>(result.ToList());
        }
    }
}