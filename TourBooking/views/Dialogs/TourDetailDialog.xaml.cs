using System;
using System.Windows;
using TourBooking.Models;

namespace TourBooking.Views.Dialogs
{
    public partial class TourDetailDialog : Window
    {
        public bool BookNowClicked { get; private set; }
        public Tour Tour { get; private set; }

        public TourDetailDialog(Tour tour)
        {
            InitializeComponent();
            Tour = tour;
            PopulateData();
        }

        private void PopulateData()
        {
            if (Tour == null) return;

            txtTourCode.Text = Tour.TourCode;
            txtTourName.Text = Tour.TourName;
            txtDestination.Text = Tour.Destination;
            txtDepartureDate.Text = Tour.DepartureDate.ToString("dd/MM/yyyy");
            txtReturnDate.Text = Tour.ReturnDate.ToString("dd/MM/yyyy");
            txtPriceAdult.Text = $"{Tour.PriceAdult:N0} đ";
            txtPriceChild.Text = $"{Tour.PriceChild:N0} đ";
            txtAvailableSlots.Text = $"{Tour.AvailableSlots} / {Tour.TotalSlots} chỗ";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            BookNowClicked = false;
            this.DialogResult = false;
            this.Close();
        }

        private void BtnBook_Click(object sender, RoutedEventArgs e)
        {
            BookNowClicked = true;
            this.DialogResult = true;
            this.Close();
        }
    }
}
