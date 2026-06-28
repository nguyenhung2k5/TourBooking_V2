using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;
using TourBooking.Data;
using TourBooking.Models;
using TourBooking.Services;

namespace TourBooking.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private int _totalTours;
        private int _activeTours;
        private int _todayBookings;
        private int _myBookingCount;
        private decimal _todayRevenue;
        private decimal _monthRevenue;
        private string _staffName;
        private string _lastError;

        
        private int _currentPage = 1;
        private int _totalPages = 1;
        private const int PageSize = 5;
        private int _totalBookingCount = 0;

        
        private decimal _myMonthRevenue;
        private int _myBookingTarget = 20; 

        public DashboardViewModel()
        {
            RecentBookings = new ObservableCollection<BookingListItemViewModel>();
            Notifications = new ObservableCollection<string>();
            LoadCommand = new RelayCommand(_ => Load());
            PrevPageCommand = new RelayCommand(_ => PreviousPage(), _ => CanPreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage(), _ => CanNextPage());
        }

        public int TotalTours
        {
            get { return _totalTours; }
            private set { SetProperty(ref _totalTours, value); }
        }

        public int ActiveTours
        {
            get { return _activeTours; }
            private set { SetProperty(ref _activeTours, value); }
        }

        public int TodayBookings
        {
            get { return _todayBookings; }
            private set { SetProperty(ref _todayBookings, value); }
        }

        public int MyBookingCount
        {
            get { return _myBookingCount; }
            private set { SetProperty(ref _myBookingCount, value); }
        }

        public decimal TodayRevenue
        {
            get { return _todayRevenue; }
            private set { SetProperty(ref _todayRevenue, value); }
        }

        public decimal MonthRevenue
        {
            get { return _monthRevenue; }
            private set { SetProperty(ref _monthRevenue, value); }
        }

        public string StaffName
        {
            get { return _staffName; }
            private set { SetProperty(ref _staffName, value); }
        }

        public string LastError
        {
            get { return _lastError; }
            private set { SetProperty(ref _lastError, value); }
        }

        
        public int CurrentPage
        {
            get { return _currentPage; }
            set 
            { 
                if (SetProperty(ref _currentPage, value))
                {
                    LoadRecentBookings();
                }
            }
        }

        public int TotalPages
        {
            get { return _totalPages; }
            private set { SetProperty(ref _totalPages, value); }
        }

        
        public decimal MyMonthRevenue
        {
            get { return _myMonthRevenue; }
            private set { SetProperty(ref _myMonthRevenue, value); }
        }

        public int MyBookingTarget
        {
            get { return _myBookingTarget; }
            set { SetProperty(ref _myBookingTarget, value); }
        }

        public ObservableCollection<BookingListItemViewModel> RecentBookings { get; private set; }
        public ObservableCollection<string> Notifications { get; private set; }
        public ICommand LoadCommand { get; private set; }
        public ICommand PrevPageCommand { get; private set; }
        public ICommand NextPageCommand { get; private set; }

        public void Load()
        {
            LastError = string.Empty;
            StaffName = SessionService.CurrentStaff != null ? SessionService.CurrentStaff.FullName : string.Empty;

            try
            {
                using (var db = new AppDbContext())
                {
                    var today = DateTime.Today;
                    var tomorrow = today.AddDays(1);
                    var monthStart = new DateTime(today.Year, today.Month, 1);
                    var nextMonth = monthStart.AddMonths(1);

                    TotalTours = db.Tours.Count();
                    ActiveTours = db.Tours.Count(t => t.IsActive);

                    var bookings = db.Bookings
                        .Include(b => b.Customer)
                        .Include(b => b.Tour)
                        .Include(b => b.Staff);

                    TodayBookings = bookings.Count(b => b.BookingDate >= today && b.BookingDate < tomorrow);
                    TodayRevenue = bookings.Where(b => b.BookingDate >= today
                                                    && b.BookingDate < tomorrow
                                                    && b.Status != BookingStatus.Cancelled
                                                    && b.Status != BookingStatus.Refunded)
                                           .Select(b => (decimal?)b.TotalAmount)
                                           .Sum() ?? 0m;

                    MonthRevenue = bookings.Where(b => b.BookingDate >= monthStart
                                                    && b.BookingDate < nextMonth
                                                    && b.Status != BookingStatus.Cancelled
                                                    && b.Status != BookingStatus.Refunded)
                                           .Select(b => (decimal?)b.TotalAmount)
                                           .Sum() ?? 0m;

                    if (SessionService.CurrentStaff != null)
                    {
                        var staffId = SessionService.CurrentStaff.StaffId;
                        MyBookingCount = bookings.Count(b => b.StaffId == staffId);

                        
                        MyMonthRevenue = bookings.Where(b => b.StaffId == staffId
                                                        && b.BookingDate >= monthStart
                                                        && b.BookingDate < nextMonth
                                                        && b.Status != BookingStatus.Cancelled
                                                        && b.Status != BookingStatus.Refunded)
                                               .Select(b => (decimal?)b.TotalAmount)
                                               .Sum() ?? 0m;
                    }
                    else
                    {
                        MyBookingCount = 0;
                        MyMonthRevenue = 0m;
                    }

                    
                    _currentPage = 1;
                    LoadRecentBookings();

                    BuildNotifications(db);
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }

        private void LoadRecentBookings()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var bookingsQuery = db.Bookings
                        .Include(b => b.Customer)
                        .Include(b => b.Tour)
                        .Include(b => b.Staff);

                    _totalBookingCount = bookingsQuery.Count();

                    
                    TotalPages = (int)Math.Ceiling((double)_totalBookingCount / PageSize);
                    if (TotalPages == 0) TotalPages = 1;

                    if (CurrentPage > TotalPages) _currentPage = TotalPages;
                    if (CurrentPage < 1) _currentPage = 1;

                    int skip = (CurrentPage - 1) * PageSize;
                    var pagedBookings = bookingsQuery.OrderByDescending(b => b.BookingDate)
                                                     .Skip(skip)
                                                     .Take(PageSize)
                                                     .ToList();

                    RecentBookings.Clear();
                    foreach (var booking in pagedBookings)
                    {
                        RecentBookings.Add(ToBookingItem(booking));
                    }

                    OnPropertyChanged(nameof(CurrentPage));
                    OnPropertyChanged(nameof(TotalPages));
                    OnPropertyChanged(nameof(MyBookingCount));
                    OnPropertyChanged(nameof(MyMonthRevenue));
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }

        private void PreviousPage()
        {
            if (CanPreviousPage())
            {
                CurrentPage--;
            }
        }

        private bool CanPreviousPage()
        {
            return CurrentPage > 1;
        }

        private void NextPage()
        {
            if (CanNextPage())
            {
                CurrentPage++;
            }
        }

        private bool CanNextPage()
        {
            return CurrentPage < TotalPages;
        }

        private ICommand _showNotificationsCommand;
        public ICommand ShowNotificationsCommand
        {
            get
            {
                if (_showNotificationsCommand == null)
                {
                    _showNotificationsCommand = new RelayCommand(_ => {
                        string msg = string.Join("\n\n", Notifications);
                        System.Windows.MessageBox.Show(msg, "Thông báo hệ thống", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    });
                }
                return _showNotificationsCommand;
            }
        }

        private void BuildNotifications(AppDbContext db)
        {
            Notifications.Clear();

            if (SessionService.CurrentStaff != null)
            {
                var staffId = SessionService.CurrentStaff.StaffId;
                var today = DateTime.Today;

                
                var myBookingsToday = db.Bookings
                    .Include(b => b.Tour)
                    .Where(b => b.StaffId == staffId && b.BookingDate >= today)
                    .OrderByDescending(b => b.BookingDate)
                    .Take(5)
                    .ToList();

                foreach (var booking in myBookingsToday)
                {
                    string statusText = booking.Status == BookingStatus.Paid ? "thành công" : "đang xử lý";
                    if (booking.Status == BookingStatus.Cancelled) statusText = "đã hủy";
                    Notifications.Add($"🎉 Đặt chỗ {statusText}: Đơn {booking.BookingCode} - Tour {booking.Tour?.TourCode} lúc {booking.BookingDate:HH:mm}");
                }
            }

            
            var lowSlotTours = db.Tours.Where(t => t.IsActive && t.AvailableSlots <= 5 && t.AvailableSlots > 0).Take(3).ToList();
            foreach (var tour in lowSlotTours)
            {
                Notifications.Add($"⚠️ Tour {tour.TourCode} chỉ còn {tour.AvailableSlots} chỗ trống.");
            }

            if (Notifications.Count == 0)
            {
                Notifications.Add("Hệ thống hoạt động bình thường. Không có thông báo mới.");
            }
        }

        private static BookingListItemViewModel ToBookingItem(Booking booking)
        {
            return new BookingListItemViewModel
            {
                BookingId = booking.BookingId,
                BookingCode = booking.BookingCode,
                CustomerName = booking.Customer != null ? booking.Customer.FullName : string.Empty,
                CustomerPhone = booking.Customer != null ? booking.Customer.Phone : string.Empty,
                TourName = booking.Tour != null ? booking.Tour.TourName : string.Empty,
                TourCode = booking.Tour != null ? booking.Tour.TourCode : string.Empty,
                StaffName = booking.Staff != null ? booking.Staff.FullName : string.Empty,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,
                StatusText = booking.Status.ToString()
            };
        }
    }
}
