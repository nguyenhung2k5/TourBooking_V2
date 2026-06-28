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
using System.Collections.Generic;

namespace TourBooking.ViewModels
{
    public class ChartBarInfo
    {
        public double Height { get; set; }
        public string Color { get; set; }
    }

    public class AdminCustomerViewModel : BaseViewModel
    {
        private ObservableCollection<Customer> _allCustomers;
        private ObservableCollection<Customer> _filteredCustomers;
        private string _searchQuery;
        private int _totalCustomersCount;
        private decimal _averageRevenue;

        public int TotalCustomersCount { get => _totalCustomersCount; set { _totalCustomersCount = value; OnPropertyChanged(); } }
        public decimal AverageRevenue { get => _averageRevenue; set { _averageRevenue = value; OnPropertyChanged(); } }

        public ObservableCollection<Customer> FilteredCustomers
        {
            get => _filteredCustomers;
            set { _filteredCustomers = value; OnPropertyChanged(); }
        }
        
        public ObservableCollection<string> RecentActivities { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<ChartBarInfo> MonthlyChartData { get; set; } = new ObservableCollection<ChartBarInfo>();

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                ExecuteSearch();
            }
        }

        public ICommand ExportCsvCommand { get; }

        public AdminCustomerViewModel()
        {
            ExportCsvCommand = new RelayCommand<object>(obj => {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Tập tin CSV (*.csv)|*.csv|Tất cả tập tin (*.*)|*.*",
                    FileName = $"BaoCao_QuanLyKhachHang_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    Title = "Chọn nơi lưu file báo cáo Khách hàng"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var reportService = new ReportService();
                    if (reportService.ExportReportToFile("Customer", saveFileDialog.FileName))
                    {
                        var result = MessageBox.Show($"Xuất báo cáo Khách hàng thành công!\nFile đã được lưu tại:\n{saveFileDialog.FileName}\n\nBạn có muốn mở file ngay không?", "Thành công", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (result == MessageBoxResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                        }
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra khi xuất báo cáo Khách hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    var customerList = context.Customers.ToList();

                    TotalCustomersCount = customerList.Count;
                    
                    var allBookings = context.Bookings.Include(b => b.Tour).Include(b => b.Customer).ToList();
                    
                    if (customerList.Count > 0)
                    {
                        AverageRevenue = allBookings.Where(b => b.Status == BookingStatus.Paid).Sum(b => b.TotalAmount) / customerList.Count;
                    }
                    else
                    {
                        AverageRevenue = 0;
                    }

                    _allCustomers = new ObservableCollection<Customer>(customerList);
                    FilteredCustomers = new ObservableCollection<Customer>(customerList);

                    // Load Recent Activities
                    var recentBookings = allBookings.OrderByDescending(b => b.BookingDate).Take(3).ToList();
                    RecentActivities.Clear();
                    foreach (var b in recentBookings)
                    {
                        TimeSpan diff = DateTime.Now - b.BookingDate;
                        string timeAgo = "";
                        if (diff.TotalMinutes < 60) timeAgo = $"{(int)diff.TotalMinutes} phút trước";
                        else if (diff.TotalHours < 24) timeAgo = $"{(int)diff.TotalHours} giờ trước";
                        else timeAgo = $"{(int)diff.TotalDays} ngày trước";
                        
                        RecentActivities.Add($"• {b.Customer?.FullName} vừa đặt {b.Tour?.TourName} ({timeAgo})");
                    }

                    // Load Chart Data (Customer growth per month for the last 10 months)
                    MonthlyChartData.Clear();
                    var groupedByMonth = customerList.GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                        .ToDictionary(g => g.Key, g => g.Count());
                    
                    var currentDate = DateTime.Now;
                    var monthsData = new List<int>();
                    for (int i = 9; i >= 0; i--)
                    {
                        var d = currentDate.AddMonths(-i);
                        var key = new { Year = d.Year, Month = d.Month };
                        monthsData.Add(groupedByMonth.ContainsKey(key) ? groupedByMonth[key] : 0);
                    }
                    
                    int maxCount = monthsData.Max();
                    if (maxCount == 0) maxCount = 1; // Prevent division by zero
                    
                    for (int i = 0; i < monthsData.Count; i++)
                    {
                        double height = ((double)monthsData[i] / maxCount) * 150;
                        if (height < 10) height = 10; // Minimum height for visibility
                        MonthlyChartData.Add(new ChartBarInfo 
                        { 
                            Height = height, 
                            Color = (i == monthsData.Count - 1) ? "#1D3D8F" : "#EAECF0" 
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối bảng Khách hàng: {ex.Message}", "Lỗi Hệ Thống");
                _allCustomers = new ObservableCollection<Customer>();
                FilteredCustomers = new ObservableCollection<Customer>();
            }
        }

        private void ExecuteSearch()
        {
            if (_allCustomers == null) return;

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredCustomers = new ObservableCollection<Customer>(_allCustomers);
                return;
            }

            var results = _allCustomers.Where(c =>
                (c.FullName != null && c.FullName.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.Email != null && c.Email.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.NationalId != null && c.NationalId.Contains(SearchQuery)) ||
                (c.Phone != null && c.Phone.Contains(SearchQuery)));

            FilteredCustomers = new ObservableCollection<Customer>(results.ToList());
        }
    }
}