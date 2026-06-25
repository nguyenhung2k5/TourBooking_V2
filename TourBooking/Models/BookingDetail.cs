using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourBooking.Models
{
    public class BookingDetail
    {
        [Key]
        public int DetailId { get; set; }

        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }

        public PassengerType PassengerType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}