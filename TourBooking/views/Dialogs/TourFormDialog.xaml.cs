using System;
using System.Windows;
using TourBooking.Data;
using TourBooking.Models;

namespace TourBooking.Views.Dialogs
{
    public partial class TourFormDialog : Window
    {
        public bool IsSuccess { get; private set; }
        private readonly Tour _tourToEdit;

        public TourFormDialog(Tour tourToEdit = null)
        {
            InitializeComponent();
            _tourToEdit = tourToEdit;

            if (_tourToEdit != null)
            {
                txtHeaderTitle.Text = "Cập nhật thông tin Tour du lịch";
                txtTourCode.Text = _tourToEdit.TourCode;
                txtTourName.Text = _tourToEdit.TourName;
                txtDestination.Text = _tourToEdit.Destination;
                dpDepartureDate.SelectedDate = _tourToEdit.DepartureDate;
                dpReturnDate.SelectedDate = _tourToEdit.ReturnDate;
                txtPriceAdult.Text = _tourToEdit.PriceAdult.ToString();
                txtPriceChild.Text = _tourToEdit.PriceChild.ToString();
                txtTotalSlots.Text = _tourToEdit.TotalSlots.ToString();
            }
            else
            {
                txtHeaderTitle.Text = "Thêm mới Tour du lịch";
                dpDepartureDate.SelectedDate = DateTime.Today.AddDays(7);
                dpReturnDate.SelectedDate = DateTime.Today.AddDays(10);
                txtTourCode.Text = "TOUR" + new Random().Next(100, 999);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTourCode.Text) || string.IsNullOrWhiteSpace(txtTourName.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mã tour và tên tour!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPriceAdult.Text.Trim(), out decimal priceAdult) ||
                !decimal.TryParse(txtPriceChild.Text.Trim(), out decimal priceChild) ||
                !int.TryParse(txtTotalSlots.Text.Trim(), out int totalSlots))
            {
                MessageBox.Show("Giá vé hoặc số chỗ không hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    if (_tourToEdit != null)
                    {
                        var dbTour = context.Tours.Find(_tourToEdit.TourId);
                        if (dbTour != null)
                        {
                            dbTour.TourCode = txtTourCode.Text.Trim();
                            dbTour.TourName = txtTourName.Text.Trim();
                            dbTour.Destination = txtDestination.Text.Trim();
                            dbTour.DepartureDate = dpDepartureDate.SelectedDate ?? dbTour.DepartureDate;
                            dbTour.ReturnDate = dpReturnDate.SelectedDate ?? dbTour.ReturnDate;
                            dbTour.PriceAdult = priceAdult;
                            dbTour.PriceChild = priceChild;
                            dbTour.TotalSlots = totalSlots;
                        }
                    }
                    else
                    {
                        var tour = new Tour
                        {
                            TourCode = txtTourCode.Text.Trim(),
                            TourName = txtTourName.Text.Trim(),
                            Destination = txtDestination.Text.Trim(),
                            DepartureDate = dpDepartureDate.SelectedDate ?? DateTime.Today.AddDays(7),
                            ReturnDate = dpReturnDate.SelectedDate ?? DateTime.Today.AddDays(10),
                            PriceAdult = priceAdult,
                            PriceChild = priceChild,
                            TotalSlots = totalSlots,
                            AvailableSlots = totalSlots,
                            IsActive = true
                        };
                        context.Tours.Add(tour);
                    }
                    context.SaveChanges();
                }

                MessageBox.Show("Lưu thông tin Tour du lịch thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                IsSuccess = true;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
