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
using TourBooking.Views.Dialogs;

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

        private bool _isDashboardSelected = true;
        public bool IsDashboardSelected { get => _isDashboardSelected; set { _isDashboardSelected = value; OnPropertyChanged(); } }

        private bool _isTourSelected;
        public bool IsTourSelected { get => _isTourSelected; set { _isTourSelected = value; OnPropertyChanged(); } }

        private bool _isBookingSelected;
        public bool IsBookingSelected { get => _isBookingSelected; set { _isBookingSelected = value; OnPropertyChanged(); } }

        private bool _isStaffSelected;
        public bool IsStaffSelected { get => _isStaffSelected; set { _isStaffSelected = value; OnPropertyChanged(); } }

        private bool _isCustomerSelected;
        public bool IsCustomerSelected { get => _isCustomerSelected; set { _isCustomerSelected = value; OnPropertyChanged(); } }

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
        public ICommand QuickAddTourCommand { get; }
        public ICommand QuickAddStaffCommand { get; }
        public ICommand ShowNotificationsCommand { get; }

        public AdminDashboardViewModel()
        {
            NavDashboardCommand = new RelayCommand<object>(obj => { SelectTab("Dashboard"); CurrentViewModel = null; });
            NavTourCommand = new RelayCommand<object>(obj => { SelectTab("Tour"); CurrentViewModel = new AdminTourViewModel(); });
            NavBookingCommand = new RelayCommand<object>(obj => { SelectTab("Booking"); CurrentViewModel = new AdminBookingsViewModel(); });
            NavStaffCommand = new RelayCommand<object>(obj => { SelectTab("Staff"); CurrentViewModel = new AdminStaffViewModel(); });
            NavCustomerCommand = new RelayCommand<object>(obj => { SelectTab("Customer"); CurrentViewModel = new AdminCustomerViewModel(); });

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

            ExportReportCommand = new RelayCommand<object>(obj => {
                var dialog = new ReportExportDialog { Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() };
                dialog.ShowDialog();
            });

            QuickAddTourCommand = new RelayCommand<object>(obj => {
                SelectTab("Tour");
                var tourVM = new AdminTourViewModel();
                CurrentViewModel = tourVM;
                tourVM.AddTourCommand.Execute(null);
            });

            QuickAddStaffCommand = new RelayCommand<object>(obj => {
                SelectTab("Staff");
                var staffVM = new AdminStaffViewModel();
                CurrentViewModel = staffVM;
                staffVM.AddStaffCommand.Execute(null);
            });

            CreateTourCommand = QuickAddTourCommand;
            ShowNotificationsCommand = new RelayCommand<object>(obj => MessageBox.Show("Bạn không có thông báo mới nào.", "Hộp thư thông báo", MessageBoxButton.OK, MessageBoxImage.Information));

            CurrentViewModel = null;
            CurrentAdminName = SessionService.CurrentStaff != null ? $"{SessionService.CurrentStaff.FullName} (Admin)" : "Admin";

            LoadDashboardSummary();
        }

        private void SelectTab(string tab)
        {
            IsDashboardSelected = tab == "Dashboard";
            IsTourSelected = tab == "Tour";
            IsBookingSelected = tab == "Booking";
            IsStaffSelected = tab == "Staff";
            IsCustomerSelected = tab == "Customer";
        }

        private void LoadDashboardSummary()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var allBookings = context.Bookings.Include(b => b.Customer).Include(b => b.Tour).ToList();
                    TotalBookings = allBookings.Count;
                    TotalRevenue = allBookings.Where(b => b.Status == BookingStatus.Paid).Sum(b => b.TotalAmount);

                    var allTours = context.Tours.ToList();
                    ActiveTours = allTours.Count(t => t.IsActive);

                    var recent = allBookings.OrderByDescending(b => b.BookingDate).Take(5).ToList();
                    RecentBookings = new ObservableCollection<Booking>(recent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu Dashboard: {ex.Message}", "Lỗi");
            }
        }
    }
}