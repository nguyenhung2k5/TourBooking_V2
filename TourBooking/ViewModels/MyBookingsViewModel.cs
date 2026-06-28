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
    public class MyBookingsViewModel : ViewModelBase
    {
        private BookingListItemViewModel _selectedBooking;
        private BookingStatus? _selectedStatus;
        private string _keyword;
        private string _lastError;

        public MyBookingsViewModel()
        {
            Bookings = new ObservableCollection<BookingListItemViewModel>();
            StatusFilters = new ObservableCollection<BookingStatus?>
            {
                null,
                BookingStatus.Pending,
                BookingStatus.Paid,
                BookingStatus.Cancelled,
                BookingStatus.Refunded
            };

            LoadCommand = new RelayCommand(_ => Load());
            SearchCommand = new RelayCommand(_ => Load());
            ClearFilterCommand = new RelayCommand(_ => ClearFilter());
            CancelBookingCommand = new RelayCommand(obj => CancelBooking(obj as BookingListItemViewModel));
            PrintInvoiceCommand = new RelayCommand(obj => PrintInvoice(obj as BookingListItemViewModel));
        }

        public ICommand CancelBookingCommand { get; private set; }
        public ICommand PrintInvoiceCommand { get; private set; }

        public ObservableCollection<BookingListItemViewModel> Bookings { get; private set; }
        public ObservableCollection<BookingStatus?> StatusFilters { get; private set; }

        public BookingListItemViewModel SelectedBooking
        {
            get { return _selectedBooking; }
            set { SetProperty(ref _selectedBooking, value); }
        }

        public BookingStatus? SelectedStatus
        {
            get { return _selectedStatus; }
            set 
            { 
                if (SetProperty(ref _selectedStatus, value))
                {
                    Load();
                }
            }
        }

        public string Keyword
        {
            get { return _keyword; }
            set { SetProperty(ref _keyword, value); }
        }

        public string LastError
        {
            get { return _lastError; }
            private set { SetProperty(ref _lastError, value); }
        }

        public int BookingCount
        {
            get { return Bookings.Count; }
        }

        public int PaidBookingCount
        {
            get { return Bookings.Count(b => b.Status == BookingStatus.Paid); }
        }

        public int PendingBookingCount
        {
            get { return Bookings.Count(b => b.Status == BookingStatus.Pending); }
        }

        public decimal TotalRevenue
        {
            get { return Bookings.Sum(b => b.TotalAmount); }
        }

        public ICommand LoadCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand ClearFilterCommand { get; private set; }

        public void Load()
        {
            LastError = string.Empty;
            Bookings.Clear();

            if (SessionService.CurrentStaff == null)
            {
                OnPropertyChanged(nameof(BookingCount));
                OnPropertyChanged(nameof(PaidBookingCount));
                OnPropertyChanged(nameof(PendingBookingCount));
                OnPropertyChanged(nameof(TotalRevenue));
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var staffId = SessionService.CurrentStaff.StaffId;
                    var query = db.Bookings
                        .Include(b => b.Customer)
                        .Include(b => b.Tour)
                        .Include(b => b.Staff)
                        .Where(b => b.StaffId == staffId);

                    if (SelectedStatus.HasValue)
                    {
                        var status = SelectedStatus.Value;
                        query = query.Where(b => b.Status == status);
                    }

                    if (!string.IsNullOrWhiteSpace(Keyword))
                    {
                        var keyword = Keyword.Trim();
                        query = query.Where(b => b.BookingCode.Contains(keyword)
                                              || b.Customer.FullName.Contains(keyword)
                                              || b.Customer.Phone.Contains(keyword)
                                              || b.Tour.TourName.Contains(keyword)
                                              || b.Tour.TourCode.Contains(keyword));
                    }

                    foreach (var booking in query.OrderByDescending(b => b.BookingDate).ToList())
                    {
                        Bookings.Add(ToBookingItem(booking));
                    }

                    OnPropertyChanged(nameof(BookingCount));
                    OnPropertyChanged(nameof(PaidBookingCount));
                    OnPropertyChanged(nameof(PendingBookingCount));
                    OnPropertyChanged(nameof(TotalRevenue));
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }

        private void ClearFilter()
        {
            Keyword = string.Empty;
            SelectedStatus = null;
            Load();
        }

        private void CancelBooking(BookingListItemViewModel item)
        {
            if (item == null) return;
            if (item.Status == BookingStatus.Cancelled || item.Status == BookingStatus.Refunded)
            {
                System.Windows.MessageBox.Show("Đơn hàng này đã được hủy hoặc hoàn tiền trước đó.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            if (System.Windows.MessageBox.Show($"Bạn có thực sự muốn hủy đơn hàng {item.BookingCode} không?", "Xác nhận hủy", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    BookingService bookingService = new BookingService();
                    if (bookingService.CancelBooking(item.BookingId))
                    {
                        System.Windows.MessageBox.Show("Hủy đơn hàng thành công!", "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                        Load();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Hủy đơn hàng thất bại.", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Lỗi: " + ex.Message, "Lỗi hệ thống", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void PrintInvoice(BookingListItemViewModel item)
        {
            if (item == null) return;

            try
            {
                using (var db = new AppDbContext())
                {
                    var booking = db.Bookings
                        .Include(b => b.Tour)
                        .Include(b => b.Customer)
                        .Include(b => b.Staff)
                        .FirstOrDefault(b => b.BookingId == item.BookingId);

                    if (booking == null)
                    {
                        System.Windows.MessageBox.Show("Không tìm thấy thông tin đơn đặt chỗ.", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }

                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "PDF files (*.pdf)|*.pdf",
                        FileName = $"Invoice_{booking.BookingCode}.pdf"
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        PdfService pdfService = new PdfService();
                        pdfService.GenerateInvoice(booking, saveFileDialog.FileName);
                        System.Windows.MessageBox.Show("Xuất hóa đơn thành công!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Lỗi xuất hóa đơn: " + ex.Message, "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
