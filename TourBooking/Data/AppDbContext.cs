using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TourBooking.Helpers;
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

        public static void EnsureSeedData()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    string passHash = PasswordHasher.Hash("123456");

                    var sampleStaffs = new List<Staff>
                    {
                        new Staff { StaffCode = "NV01", FullName = "Nguyễn Trọng Hùng", Username = "admin", PasswordHash = passHash, Role = UserRole.Admin, IsActive = true },
                        new Staff { StaffCode = "NV02", FullName = "Trần Thị Mai", Username = "admin2", PasswordHash = passHash, Role = UserRole.Admin, IsActive = true },
                        new Staff { StaffCode = "NV03", FullName = "Lê Hồng Nhung", Username = "nv_nhung", PasswordHash = passHash, Role = UserRole.Staff, IsActive = true },
                        new Staff { StaffCode = "NV04", FullName = "Phạm Minh Tuấn", Username = "nv_tuan", PasswordHash = passHash, Role = UserRole.Staff, IsActive = true },
                        new Staff { StaffCode = "NV05", FullName = "Hoàng Anh Nam", Username = "nv_nam", PasswordHash = passHash, Role = UserRole.Staff, IsActive = true }
                    };

                    bool addedAny = false;
                    foreach (var s in sampleStaffs)
                    {
                        if (!db.Staffs.Any(x => x.Username.ToLower() == s.Username.ToLower()))
                        {
                            db.Staffs.Add(s);
                            addedAny = true;
                        }
                    }

                    if (addedAny)
                    {
                        db.SaveChanges();
                    }
                }
            }
            catch { }
        }
    }

    public class AppDbInitializer : CreateDatabaseIfNotExists<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            string defaultPasswordHash = PasswordHasher.Hash("123456");

            context.Staffs.Add(new Staff { StaffCode = "NV01", FullName = "Nguyễn Trọng Hùng", Username = "admin", PasswordHash = defaultPasswordHash, Role = UserRole.Admin, IsActive = true });
            context.Staffs.Add(new Staff { StaffCode = "NV02", FullName = "Trần Thị Mai", Username = "admin2", PasswordHash = defaultPasswordHash, Role = UserRole.Admin, IsActive = true });
            context.Staffs.Add(new Staff { StaffCode = "NV03", FullName = "Lê Hồng Nhung", Username = "nv_nhung", PasswordHash = defaultPasswordHash, Role = UserRole.Staff, IsActive = true });
            context.Staffs.Add(new Staff { StaffCode = "NV04", FullName = "Phạm Minh Tuấn", Username = "nv_tuan", PasswordHash = defaultPasswordHash, Role = UserRole.Staff, IsActive = true });
            context.Staffs.Add(new Staff { StaffCode = "NV05", FullName = "Hoàng Anh Nam", Username = "nv_nam", PasswordHash = defaultPasswordHash, Role = UserRole.Staff, IsActive = true });

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