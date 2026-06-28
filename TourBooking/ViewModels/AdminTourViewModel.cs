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
using TourBooking.Views.Dialogs;

namespace TourBooking.ViewModels
{
    public class AdminTourViewModel : BaseViewModel
    {
        private int _totalTours;
        private int _activeTours;
        private double _fillRate;
        private int _inactiveTours;
        private ObservableCollection<Tour> _toursList;
        private ObservableCollection<Tour> _upcomingTours;
        private ObservableCollection<string> _systemNotifications;

        public int TotalTours { get => _totalTours; set { _totalTours = value; OnPropertyChanged(); } }
        public int ActiveTours { get => _activeTours; set { _activeTours = value; OnPropertyChanged(); } }
        public double FillRate { get => _fillRate; set { _fillRate = value; OnPropertyChanged(); } }
        public int InactiveTours { get => _inactiveTours; set { _inactiveTours = value; OnPropertyChanged(); } }
        public ObservableCollection<Tour> ToursList { get => _toursList; set { _toursList = value; OnPropertyChanged(); } }
        public ObservableCollection<Tour> UpcomingTours { get => _upcomingTours; set { _upcomingTours = value; OnPropertyChanged(); } }
        public ObservableCollection<string> SystemNotifications { get => _systemNotifications; set { _systemNotifications = value; OnPropertyChanged(); } }

        public ICommand AddTourCommand { get; }
        public ICommand EditTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand DetailTourCommand { get; }
        public ICommand ExportCsvCommand { get; }

        public AdminTourViewModel()
        {
            DetailTourCommand = new RelayCommand<Tour>(tour => {
                if (tour == null) return;
                var dialog = new TourDetailDialog(tour) { Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() };
                dialog.ShowDialog();
            });

            AddTourCommand = new RelayCommand<object>(obj => {
                var dialog = new TourFormDialog { Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() };
                if (dialog.ShowDialog() == true && dialog.IsSuccess)
                {
                    LoadDataFromDatabase();
                }
            });
            EditTourCommand = new RelayCommand<Tour>(tour => {
                if (tour == null) return;
                var dialog = new TourFormDialog(tour) { Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() };
                if (dialog.ShowDialog() == true && dialog.IsSuccess)
                {
                    LoadDataFromDatabase();
                }
            });
            DeleteTourCommand = new RelayCommand<Tour>(ExecuteDeleteTour);
            
            ExportCsvCommand = new RelayCommand<object>(obj => {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Tập tin CSV (*.csv)|*.csv|Tất cả tập tin (*.*)|*.*",
                    FileName = $"BaoCao_QuanLyTour_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    Title = "Chọn nơi lưu file báo cáo Tour du lịch"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var reportService = new ReportService();
                    if (reportService.ExportReportToFile("Tour", saveFileDialog.FileName))
                    {
                        var result = MessageBox.Show($"Xuất báo cáo Tour thành công!\nFile đã được lưu tại:\n{saveFileDialog.FileName}\n\nBạn có muốn mở file ngay không?", "Thành công", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (result == MessageBoxResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                        }
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra khi xuất báo cáo Tour!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    var allTours = context.Tours.ToList();

                    TotalTours = allTours.Count;
                    ActiveTours = allTours.Count(t => t.IsActive);
                    InactiveTours = allTours.Count(t => !t.IsActive);

                    int totalSlots = allTours.Sum(t => t.TotalSlots);
                    int totalAvailable = allTours.Sum(t => t.AvailableSlots);
                    int totalBooked = totalSlots - totalAvailable;
                    FillRate = totalSlots > 0 ? Math.Round((double)totalBooked / totalSlots * 100, 1) : 0;

                    ToursList = new ObservableCollection<Tour>(allTours);

                    DateTime today = DateTime.Today;
                    var upcoming = context.Tours
                        .Where(t => t.DepartureDate >= today && t.IsActive)
                        .OrderBy(t => t.DepartureDate)
                        .Take(2)
                        .ToList();
                    UpcomingTours = new ObservableCollection<Tour>(upcoming);

                    
                    var recentBookings = context.Bookings
                        .Include(b => b.Customer)
                        .Include(b => b.Tour)
                        .OrderByDescending(b => b.BookingDate)
                        .Take(8)
                        .ToList();

                    var notifs = new ObservableCollection<string>();
                    if (recentBookings.Any())
                    {
                        foreach (var b in recentBookings)
                        {
                            string custName = b.Customer != null ? b.Customer.FullName : "Khách hàng";
                            string tourName = b.Tour != null ? b.Tour.TourName : "Tour";
                            notifs.Add($"[Mới] Khách {custName} vừa đặt đơn {b.BookingCode} cho {tourName} vào lúc {b.BookingDate:HH:mm dd/MM}.");
                        }
                    }
                    else
                    {
                        notifs.Add("Hiện chưa có thông báo mới nào về giao dịch đặt tour.");
                    }
                    SystemNotifications = notifs;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối CSDL: {ex.Message}", "Lỗi Hệ Thống");
            }
        }

        private void ExecuteDeleteTour(Tour tour)
        {
            if (tour == null) return;
            if (MessageBox.Show($"Xác nhận xóa Tour: {tour.TourName}?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        var dbTour = context.Tours.Find(tour.TourId);
                        if (dbTour != null)
                        {
                            context.Tours.Remove(dbTour);
                            context.SaveChanges();
                            LoadDataFromDatabase();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}");
                }
            }
        }
    }
}