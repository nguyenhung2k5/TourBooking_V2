using System;
using System.Windows;
using TourBooking.Services;

namespace TourBooking.Views.Dialogs
{
    public partial class QrPaymentDialog : Window
    {
        public bool PaymentConfirmed { get; private set; }

        public QrPaymentDialog(string bookingCode, decimal amount)
        {
            InitializeComponent();
            txtAmount.Text = $"Số tiền: {amount:N0} đ";
            txtBookingCode.Text = $"Nội dung: {bookingCode}";
            
            QrService qrService = new QrService();
            var qrImage = qrService.GeneratePaymentQr(bookingCode, amount);
            if (qrImage != null)
            {
                imgQrCode.Source = qrImage;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            PaymentConfirmed = false;
            this.DialogResult = false;
            this.Close();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            PaymentConfirmed = true;
            this.DialogResult = true;
            this.Close();
        }
    }
}
