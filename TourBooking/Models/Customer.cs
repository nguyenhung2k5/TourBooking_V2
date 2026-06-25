using System;
using System.ComponentModel.DataAnnotations;

namespace TourBooking.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string NationalId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}