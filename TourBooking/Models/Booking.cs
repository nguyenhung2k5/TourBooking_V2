using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourBooking.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        [StringLength(50)]
        public string BookingCode { get; set; }

        public int TourId { get; set; }
        [ForeignKey("TourId")]
        public virtual Tour Tour { get; set; }

        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public int StaffId { get; set; }
        [ForeignKey("StaffId")]
        public virtual Staff Staff { get; set; }

        public BookingStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;

        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}