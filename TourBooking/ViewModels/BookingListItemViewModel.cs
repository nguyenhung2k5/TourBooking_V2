using System;
using TourBooking.Models;

namespace TourBooking.ViewModels
{
    public class BookingListItemViewModel
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string TourName { get; set; }
        public string TourCode { get; set; }
        public string StaffName { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case BookingStatus.Pending: return "Chờ thanh toán";
                    case BookingStatus.Paid: return "Đã thanh toán";
                    case BookingStatus.Cancelled: return "Đã hủy";
                    case BookingStatus.Refunded: return "Hoàn tiền";
                    default: return Status.ToString();
                }
            }
            set { }
        }

        public string StatusColor
        {
            get
            {
                switch (Status)
                {
                    case BookingStatus.Paid: return "#12B76A";
                    case BookingStatus.Pending: return "#F79009";
                    case BookingStatus.Cancelled: return "#F04438";
                    case BookingStatus.Refunded: return "#98A2B3";
                    default: return "#344054";
                }
            }
        }

        public string BadgeBgColor
        {
            get
            {
                switch (Status)
                {
                    case BookingStatus.Paid: return "#E6F4EA";
                    case BookingStatus.Pending: return "#FEF3EB";
                    case BookingStatus.Cancelled: return "#FEE4E2";
                    case BookingStatus.Refunded: return "#F2F4F7";
                    default: return "#F2F4F7";
                }
            }
        }

        public string StatusBg => BadgeBgColor;
    }
}
