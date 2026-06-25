using System.ComponentModel.DataAnnotations;

namespace TourBooking.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

        [Required]
        [StringLength(50)]
        public string StaffCode { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
    }
}