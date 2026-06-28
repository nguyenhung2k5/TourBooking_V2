using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourBooking.Models
{
    public class Tour
    {
        [Key]
        public int TourId { get; set; }

        [Required]
        [StringLength(50)]
        public string TourCode { get; set; }

        [Required]
        [StringLength(200)]
        public string TourName { get; set; }

        [Required]
        [StringLength(100)]
        public string Destination { get; set; }

        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal PriceAdult { get; set; }
        public decimal PriceChild { get; set; }
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public bool IsActive { get; set; } = true;

        [NotMapped]
        public string StatusColor
        {
            get
            {
                if (AvailableSlots <= 0) return "#F04438"; // Red
                if (AvailableSlots <= 5) return "#F79009"; // Orange
                return "#12B76A"; // Green
            }
        }
    }
}