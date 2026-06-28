using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TourBooking.Data;
using TourBooking.Models;

namespace TourBooking.ViewModels
{
    public class AdminBookingsViewModel : BaseViewModel
    {
        private ObservableCollection<Booking> _allBookings;
        private ObservableCollection<Booking> _filteredBookings;
        private string _searchQuery;
        private string _selectedStatus = "Tất cả";
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
        public DateTime? SelectedDate { get => _selectedDate; set { _selectedDate = value; OnPropertyChanged(); } }

        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string> { "Tất cả", "Pending", "Paid", "Cancelled", "Refunded" };

        public ICommand FilterCommand { get; }

        public AdminBookingsViewModel()
        {
            FilterCommand = new RelayCommand<object>(ExecuteFilter);
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
                        .ToList();

                    TotalBookingsCount = bookingsList.Count;
                    TotalRevenueSum = bookingsList.Where(b => b.Status == BookingStatus.Paid).Sum(b => b.TotalAmount);

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
                result = result.Where(b => b.Status.ToString().Equals(SelectedStatus, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedDate.HasValue)
            {
                result = result.Where(b => b.BookingDate.Date == SelectedDate.Value.Date);
            }

            FilteredBookings = new ObservableCollection<Booking>(result.ToList());
        }
    }
}