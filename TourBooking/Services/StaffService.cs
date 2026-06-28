using System;
using System.Linq;
using TourBooking.Data;
using TourBooking.Helpers;
using TourBooking.Models;

namespace TourBooking.Services
{
    public class StaffService
    {
        public Staff Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            using (var db = new AppDbContext())
            {
                string passwordHash = PasswordHasher.Hash(password);
                var staff = db.Staffs.FirstOrDefault(s => s.Username == username
                                                       && (s.PasswordHash == passwordHash || s.PasswordHash == password)
                                                       && s.IsActive == true);
                return staff;
            }
        }
    }
}
