using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TourBooking.Data;
using TourBooking.Models;
using TourBooking.Services;

namespace TourBooking.ViewModels
{
    public class BookingViewModel : ViewModelBase
    {
        private Tour _selectedTour;
        private string _customerName;
        private string _customerPhone;
        private string _customerEmail;
        private string _customerNationalId;
        private int _adultQuantity = 1;
        private int _childQuantity;
        private PaymentMethod _selectedPaymentMethod;
        private string _lastError;
        private string _successMessage;

        public BookingViewModel()
        {
            Tours = new ObservableCollection<Tour>();
            PaymentMethods = new ObservableCollection<PaymentMethod>
            {
                PaymentMethod.Cash,
                PaymentMethod.Transfer
            };

            LoadCommand = new RelayCommand(_ => Load());
            CreateBookingCommand = new RelayCommand(_ => CreateBooking(), _ => CanCreateBooking());
            ClearCommand = new RelayCommand(_ => ClearForm());
            IncreaseAdultCommand = new RelayCommand(_ => AdultQuantity++);
            DecreaseAdultCommand = new RelayCommand(_ => AdultQuantity--, _ => AdultQuantity > 0);
            IncreaseChildCommand = new RelayCommand(_ => ChildQuantity++);
            DecreaseChildCommand = new RelayCommand(_ => ChildQuantity--, _ => ChildQuantity > 0);
        }

        public ObservableCollection<Tour> Tours { get; private set; }
        public ObservableCollection<PaymentMethod> PaymentMethods { get; private set; }

        public Tour SelectedTour
        {
            get { return _selectedTour; }
            set
            {
                if (SetProperty(ref _selectedTour, value))
                {
                    OnPropertyChanged(nameof(TotalAmount));
                    OnPropertyChanged(nameof(AvailableSlotsText));
                    OnPropertyChanged(nameof(AdultTotalAmount));
                    OnPropertyChanged(nameof(ChildTotalAmount));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                if (SetProperty(ref _customerName, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string CustomerPhone
        {
            get { return _customerPhone; }
            set
            {
                if (SetProperty(ref _customerPhone, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string CustomerEmail
        {
            get { return _customerEmail; }
            set { SetProperty(ref _customerEmail, value); }
        }

        public string CustomerNationalId
        {
            get { return _customerNationalId; }
            set { SetProperty(ref _customerNationalId, value); }
        }

        public int AdultQuantity
        {
            get { return _adultQuantity; }
            set
            {
                var normalized = value < 0 ? 0 : value;
                if (SetProperty(ref _adultQuantity, normalized))
                {
                    OnPropertyChanged(nameof(TotalPassengers));
                    OnPropertyChanged(nameof(TotalAmount));
                    OnPropertyChanged(nameof(AdultTotalAmount));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public int ChildQuantity
        {
            get { return _childQuantity; }
            set
            {
                var normalized = value < 0 ? 0 : value;
                if (SetProperty(ref _childQuantity, normalized))
                {
                    OnPropertyChanged(nameof(TotalPassengers));
                    OnPropertyChanged(nameof(TotalAmount));
                    OnPropertyChanged(nameof(ChildTotalAmount));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public PaymentMethod SelectedPaymentMethod
        {
            get { return _selectedPaymentMethod; }
            set 
            { 
                if (SetProperty(ref _selectedPaymentMethod, value))
                {
                    OnPropertyChanged(nameof(IsCashPayment));
                    OnPropertyChanged(nameof(IsTransferPayment));
                }
            }
        }

        public bool IsCashPayment
        {
            get { return SelectedPaymentMethod == PaymentMethod.Cash; }
            set { if (value) SelectedPaymentMethod = PaymentMethod.Cash; }
        }

        public bool IsTransferPayment
        {
            get { return SelectedPaymentMethod == PaymentMethod.Transfer; }
            set { if (value) SelectedPaymentMethod = PaymentMethod.Transfer; }
        }

        public string LastError
        {
            get { return _lastError; }
            private set { SetProperty(ref _lastError, value); }
        }

        public string SuccessMessage
        {
            get { return _successMessage; }
            private set { SetProperty(ref _successMessage, value); }
        }

        public int TotalPassengers
        {
            get { return AdultQuantity + ChildQuantity; }
        }

        public decimal AdultTotalAmount
        {
            get { return SelectedTour == null ? 0 : AdultQuantity * SelectedTour.PriceAdult; }
        }

        public decimal ChildTotalAmount
        {
            get { return SelectedTour == null ? 0 : ChildQuantity * SelectedTour.PriceChild; }
        }

        public decimal TotalAmount
        {
            get
            {
                if (SelectedTour == null)
                {
                    return 0m;
                }

                return (AdultQuantity * SelectedTour.PriceAdult) + (ChildQuantity * SelectedTour.PriceChild);
            }
        }

        public string AvailableSlotsText
        {
            get { return SelectedTour == null ? string.Empty : SelectedTour.AvailableSlots + " cho trong"; }
        }

        public ICommand LoadCommand { get; private set; }
        public ICommand CreateBookingCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand IncreaseAdultCommand { get; private set; }
        public ICommand DecreaseAdultCommand { get; private set; }
        public ICommand IncreaseChildCommand { get; private set; }
        public ICommand DecreaseChildCommand { get; private set; }

        public void Load()
        {
            LastError = string.Empty;

            try
            {
                using (var db = new AppDbContext())
                {
                    Tours.Clear();
                    foreach (var tour in db.Tours.Where(t => t.IsActive && t.AvailableSlots > 0)
                                                 .OrderBy(t => t.DepartureDate)
                                                 .ToList())
                    {
                        Tours.Add(tour);
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }

        private bool CanCreateBooking()
        {
            return SelectedTour != null
                && !string.IsNullOrWhiteSpace(CustomerName)
                && !string.IsNullOrWhiteSpace(CustomerPhone)
                && TotalPassengers > 0
                && SelectedTour.AvailableSlots >= TotalPassengers
                && SessionService.CurrentStaff != null;
        }

        private void CreateBooking()
        {
            LastError = string.Empty;
            SuccessMessage = string.Empty;

            if (!CanCreateBooking())
            {
                MessageBox.Show("Vui lòng kiểm tra thông tin booking trước khi xác nhận.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // QR payment popup check if Transfer is selected
            if (SelectedPaymentMethod == PaymentMethod.Transfer)
            {
                string tempBookingCode = "BK" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var qrDialog = new TourBooking.Views.Dialogs.QrPaymentDialog(tempBookingCode, TotalAmount);
                if (qrDialog.ShowDialog() != true)
                {
                    return; // Payment not confirmed
                }
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var tour = db.Tours.FirstOrDefault(t => t.TourId == SelectedTour.TourId);
                    if (tour == null || !tour.IsActive)
                    {
                        LastError = "Tour khong ton tai hoac da ngung hoat dong.";
                        MessageBox.Show("Tour không tồn tại hoặc đã ngừng hoạt động.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (tour.AvailableSlots < TotalPassengers)
                    {
                        LastError = "Tour khong du so cho trong.";
                        MessageBox.Show("Tour không đủ số chỗ trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var phone = CustomerPhone.Trim();
                    var customer = db.Customers.FirstOrDefault(c => c.Phone == phone);
                    if (customer == null)
                    {
                        customer = new Customer
                        {
                            FullName = CustomerName.Trim(),
                            Phone = phone,
                            Email = string.IsNullOrWhiteSpace(CustomerEmail) ? null : CustomerEmail.Trim(),
                            NationalId = string.IsNullOrWhiteSpace(CustomerNationalId) ? null : CustomerNationalId.Trim(),
                            CreatedAt = DateTime.Now
                        };
                        db.Customers.Add(customer);
                    }
                    else
                    {
                        customer.FullName = CustomerName.Trim();
                        customer.Email = string.IsNullOrWhiteSpace(CustomerEmail) ? customer.Email : CustomerEmail.Trim();
                        customer.NationalId = string.IsNullOrWhiteSpace(CustomerNationalId) ? customer.NationalId : CustomerNationalId.Trim();
                    }

                    var booking = new Booking
                    {
                        BookingCode = GenerateBookingCode(),
                        Tour = tour,
                        Customer = customer,
                        StaffId = SessionService.CurrentStaff.StaffId,
                        Status = BookingStatus.Paid,
                        TotalAmount = TotalAmount,
                        BookingDate = DateTime.Now,
                        BookingDetails = new Collection<BookingDetail>(),
                        Payments = new Collection<Payment>()
                    };

                    if (AdultQuantity > 0)
                    {
                        booking.BookingDetails.Add(new BookingDetail
                        {
                            PassengerType = PassengerType.Adult,
                            Quantity = AdultQuantity,
                            UnitPrice = tour.PriceAdult
                        });
                    }

                    if (ChildQuantity > 0)
                    {
                        booking.BookingDetails.Add(new BookingDetail
                        {
                            PassengerType = PassengerType.Child,
                            Quantity = ChildQuantity,
                            UnitPrice = tour.PriceChild
                        });
                    }

                    booking.Payments.Add(new Payment
                    {
                        Amount = TotalAmount,
                        Method = SelectedPaymentMethod,
                        PaidAt = DateTime.Now
                    });

                    tour.AvailableSlots -= TotalPassengers;
                    db.Bookings.Add(booking);
                    db.SaveChanges();

                    SuccessMessage = "Tao booking thanh cong: " + booking.BookingCode;
                    MessageBox.Show("Tạo booking thành công: " + booking.BookingCode, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Ask to print invoice
                    if (MessageBox.Show("Bạn có muốn xuất hóa đơn PDF không?", "In hóa đơn", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                        {
                            Filter = "PDF files (*.pdf)|*.pdf",
                            FileName = $"Invoice_{booking.BookingCode}.pdf"
                        };
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            try
                            {
                                PdfService pdfService = new PdfService();
                                pdfService.GenerateInvoice(booking, saveFileDialog.FileName);
                                MessageBox.Show("Xuất hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                                System.Diagnostics.Process.Start(saveFileDialog.FileName);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi xuất hóa đơn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }

                    ClearForm();
                    Load();
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                MessageBox.Show("Lỗi tạo booking: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string GenerateBookingCode()
        {
            return "BK" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private void ClearForm()
        {
            SelectedTour = null;
            CustomerName = string.Empty;
            CustomerPhone = string.Empty;
            CustomerEmail = string.Empty;
            CustomerNationalId = string.Empty;
            AdultQuantity = 1;
            ChildQuantity = 0;
            SelectedPaymentMethod = PaymentMethod.Cash;
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(TotalPassengers));
            OnPropertyChanged(nameof(AvailableSlotsText));
        }
    }
}
