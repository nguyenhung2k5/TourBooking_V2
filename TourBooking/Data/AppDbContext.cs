using System;
using System.Data.Entity;
using TourBooking.Models;

namespace TourBooking.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=AppDbContext")
        {
            Database.SetInitializer(new AppDbInitializer());
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class AppDbInitializer : CreateDatabaseIfNotExists<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            string defaultPasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92";

            context.Staffs.Add(new Staff { StaffCode = "NV01", FullName = "Nguyễn Trọng Hùng", Username = "admin", PasswordHash = defaultPasswordHash, Role = UserRole.Admin, IsActive = true });
            

            context.Tours.Add(new Tour
            {
                TourCode = "T01",
                TourName = "Hà Nội - Đà Nẵng 3N2Đ",
                Destination = "Đà Nẵng",
                DepartureDate = DateTime.Now.AddDays(10),
                ReturnDate = DateTime.Now.AddDays(12),
                PriceAdult = 3500000,
                PriceChild = 1500000,
                TotalSlots = 20,
                AvailableSlots = 20,
                IsActive = true
            });

            base.Seed(context);
        }
    }
}