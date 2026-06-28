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

        public DashboardViewModel()
        {
            RecentBookings = new ObservableCollection<BookingListItemViewModel>();
            Notifications = new ObservableCollection<string>();
            LoadCommand = new RelayCommand(_ => Load());
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

        public ObservableCollection<BookingListItemViewModel> RecentBookings { get; private set; }
        public ObservableCollection<string> Notifications { get; private set; }
        public ICommand LoadCommand { get; private set; }

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
                    }
                    else
                    {
                        MyBookingCount = 0;
                    }

                    RecentBookings.Clear();
                    foreach (var booking in bookings.OrderByDescending(b => b.BookingDate).Take(8).ToList())
                    {
                        RecentBookings.Add(ToBookingItem(booking));
                    }

                    BuildNotifications(db);
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }

        private void BuildNotifications(AppDbContext db)
        {
            Notifications.Clear();

            var lowSlotTours = db.Tours.Where(t => t.IsActive && t.AvailableSlots <= 5).Take(5).ToList();
            foreach (var tour in lowSlotTours)
            {
                Notifications.Add(tour.TourCode + " chi con " + tour.AvailableSlots + " cho.");
            }

            if (Notifications.Count == 0)
            {
                Notifications.Add("He thong dang hoat dong binh thuong.");
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
