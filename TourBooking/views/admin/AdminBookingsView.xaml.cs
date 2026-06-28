using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TourBooking.Views.Admin
{
    /// <summary>
    /// Interaction logic for AdminBookingsView.xaml
    /// </summary>
    public partial class AdminBookingsView : UserControl
    {
        public AdminBookingsView()
        {
            InitializeComponent();
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            var booking = comboBox.DataContext as TourBooking.Models.Booking;
            if (booking == null) return;

            var selectedItem = comboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null || selectedItem.Tag == null) return;

            var newStatus = (TourBooking.Models.BookingStatus)selectedItem.Tag;
            if (booking.Status == newStatus) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn thay đổi trạng thái đơn hàng {booking.BookingCode} sang '{selectedItem.Content}' không?", "Xác nhận thay đổi", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new TourBooking.Data.AppDbContext())
                    {
                        var dbBooking = context.Bookings.Include("Tour").FirstOrDefault(b => b.BookingId == booking.BookingId);
                        if (dbBooking != null)
                        {
                            var oldStatus = dbBooking.Status;
                            dbBooking.Status = newStatus;

                            // Tự động hoàn lại hoặc trừ bớt số chỗ ngồi khả dụng của Tour
                            if (dbBooking.Tour != null)
                            {
                                int totalPassengers = context.BookingDetails.Where(d => d.BookingId == dbBooking.BookingId).Sum(d => (int?)d.Quantity) ?? 0;

                                // Nếu chuyển từ hoạt động (Paid) sang hủy/hoàn -> Trả lại chỗ ngồi
                                if (oldStatus == TourBooking.Models.BookingStatus.Paid && 
                                    (newStatus == TourBooking.Models.BookingStatus.Cancelled || newStatus == TourBooking.Models.BookingStatus.Refunded))
                                {
                                    dbBooking.Tour.AvailableSlots += totalPassengers;
                                }
                                // Nếu chuyển từ hủy/hoàn (Cancelled/Refunded) sang hoạt động (Paid) -> Khấu trừ chỗ ngồi
                                else if ((oldStatus == TourBooking.Models.BookingStatus.Cancelled || oldStatus == TourBooking.Models.BookingStatus.Refunded) && 
                                         newStatus == TourBooking.Models.BookingStatus.Paid)
                                {
                                    dbBooking.Tour.AvailableSlots -= totalPassengers;
                                }
                            }

                            context.SaveChanges();
                            booking.Status = newStatus;

                            // Tải lại dữ liệu từ Database để làm mới danh sách và cập nhật tổng tiền doanh thu ở trên
                            var vm = this.DataContext as TourBooking.ViewModels.AdminBookingsViewModel;
                            if (vm != null)
                            {
                                vm.LoadDataFromDatabase();
                            }

                            MessageBox.Show("Cập nhật trạng thái đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi cập nhật trạng thái: {ex.Message}", "Lỗi");
                    comboBox.SelectionChanged -= StatusComboBox_SelectionChanged;
                    comboBox.SelectedValue = booking.Status;
                    comboBox.SelectionChanged += StatusComboBox_SelectionChanged;
                }
            }
            else
            {
                // Reset lại ComboBox về trạng thái cũ của đơn hàng
                comboBox.SelectionChanged -= StatusComboBox_SelectionChanged;
                comboBox.SelectedValue = booking.Status;
                comboBox.SelectionChanged += StatusComboBox_SelectionChanged;
            }
        }
    }
}
