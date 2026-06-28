using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TourBooking.Data;
using TourBooking.Models;
using TourBooking.Services;
using TourBooking.Views;

namespace TourBooking.ViewModels
{
    public class AdminDashboardViewModel : BaseViewModel
    {
        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set { _currentViewModel = value; OnPropertyChanged(); }
        }

        public ICommand NavDashboardCommand { get; }
        public ICommand NavTourCommand { get; }
        public ICommand NavBookingCommand { get; }
        public ICommand NavStaffCommand { get; }
        public ICommand NavCustomerCommand { get; }
        public ICommand LogoutCommand { get; }

        private string _currentAdminName;
        public string CurrentAdminName { get => _currentAdminName; set { _currentAdminName = value; OnPropertyChanged(); } }

        private decimal _totalRevenue;
        public decimal TotalRevenue { get => _totalRevenue; set { _totalRevenue = value; OnPropertyChanged(); } }

        private int _totalBookings;
        public int TotalBookings { get => _totalBookings; set { _totalBookings = value; OnPropertyChanged(); } }

        private int _activeTours;
        public int ActiveTours { get => _activeTours; set { _activeTours = value; OnPropertyChanged(); } }

        private int _pendingBookingsCount;
        public int PendingBookingsCount { get => _pendingBookingsCount; set { _pendingBookingsCount = value; OnPropertyChanged(); } }

        private int _paidBookingsCount;
        public int PaidBookingsCount { get => _paidBookingsCount; set { _paidBookingsCount = value; OnPropertyChanged(); } }

        private int _cancelledBookingsCount;
        public int CancelledBookingsCount { get => _cancelledBookingsCount; set { _cancelledBookingsCount = value; OnPropertyChanged(); } }

        private ObservableCollection<Booking> _recentBookings;
        public ObservableCollection<Booking> RecentBookings { get => _recentBookings; set { _recentBookings = value; OnPropertyChanged(); } }

        public ICommand ExportReportCommand { get; }
        public ICommand CreateTourCommand { get; }
        public ICommand ShowNotificationsCommand { get; }

        public AdminDashboardViewModel()
        {
            NavDashboardCommand = new RelayCommand<object>(obj => CurrentViewModel = null);
            NavTourCommand = new RelayCommand<object>(obj => CurrentViewModel = new AdminTourViewModel());
            NavBookingCommand = new RelayCommand<object>(obj => CurrentViewModel = new AdminBookingsViewModel());
            NavStaffCommand = new RelayCommand<object>(obj => CurrentViewModel = new AdminStaffViewModel());
            NavCustomerCommand = new RelayCommand<object>(obj => CurrentViewModel = new AdminCustomerViewModel());

            LogoutCommand = new RelayCommand<object>(obj => {
                if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    SessionService.Logout();
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();

                    Window currentWindow = obj as Window;
                    if (currentWindow == null)
                    {
                        currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is MainWindow || w.IsActive);
                    }
                    currentWindow?.Close();
                }
            });

            ExportReportCommand = new RelayCommand<object>(obj => MessageBox.Show("Đang kết xuất báo cáo hệ thống ra file Excel...", "Thông báo"));
            CreateTourCommand = new RelayCommand<object>(obj => MessageBox.Show("Tính năng Thêm Tour Đang được xây dựng!", "Thông báo"));
            ShowNotificationsCommand = new RelayCommand<object>(obj => MessageBox.Show("Bạn không có thông báo mới nào.", "Hộp thư thông báo", MessageBoxButton.OK, MessageBoxImage.Information));

            CurrentViewModel = null;
            CurrentAdminName = SessionService.CurrentStaff != null ? $"{SessionService.CurrentStaff.FullName} (Admin)" : "Admin";

            LoadDashboardSummary();
        }

        private void LoadDashboardSummary()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    TotalBookings = context.Bookings.Count();

                    TotalRevenue = context.Bookings.Any(b => b.Status == BookingStatus.Paid)
                                 ? context.Bookings.Where(b => b.Status == BookingStatus.Paid).Sum(b => b.TotalAmount)
                                 : 0;

                    ActiveTours = context.Tours.Count();

                    PendingBookingsCount = context.Bookings.Count(b => b.Status == BookingStatus.Pending);
                    PaidBookingsCount = context.Bookings.Count(b => b.Status == BookingStatus.Paid);
                    CancelledBookingsCount = context.Bookings.Count(b => b.Status == BookingStatus.Cancelled || b.Status == BookingStatus.Refunded);

                    var recentList = context.Bookings
                        .Include(b => b.Customer)
                        .Include(b => b.Tour)
                        .OrderByDescending(b => b.BookingDate)
                        .Take(5)
                        .ToList();

                    RecentBookings = new ObservableCollection<Booking>(recentList);
                }
            }
            catch (Exception)
            {
                TotalBookings = 0;
                TotalRevenue = 0;
                ActiveTours = 0;
                PendingBookingsCount = 0;
                PaidBookingsCount = 0;
                CancelledBookingsCount = 0;
                RecentBookings = new ObservableCollection<Booking>();
            }
        }
    }
}