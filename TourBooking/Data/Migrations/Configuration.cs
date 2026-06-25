namespace TourBooking.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TourBooking.Models; 

    internal sealed class Configuration : DbMigrationsConfiguration<TourBooking.Data.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TourBooking.Data.AppDbContext context)
        {
            string defaultPasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92";

            context.Staffs.AddOrUpdate(
                s => s.Username,
                new Staff { StaffCode = "NV01", FullName = "Nguyễn Trọng Hùng", Username = "admin", PasswordHash = defaultPasswordHash, Role = UserRole.Admin, IsActive = true }
            );

            context.Tours.AddOrUpdate(
                t => t.TourCode,
                new Tour { TourCode = "T01", TourName = "Hà Nội - Đà Nẵng 3N2Đ", Destination = "Đà Nẵng", DepartureDate = DateTime.Now.AddDays(10), ReturnDate = DateTime.Now.AddDays(12), PriceAdult = 3500000, PriceChild = 1500000, TotalSlots = 20, AvailableSlots = 20, IsActive = true }
            );

            context.SaveChanges();
        }
    }
}