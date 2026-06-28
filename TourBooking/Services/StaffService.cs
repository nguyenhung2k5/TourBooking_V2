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

        public RegisterResult Register(string fullName, string username, string password, UserRole role = UserRole.Staff)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new RegisterResult { Success = false, Message = "Vui lòng nhập đầy đủ thông tin!" };
            }

            using (var db = new AppDbContext())
            {
                var cleanUsername = username.Trim();
                var existing = db.Staffs.FirstOrDefault(s => s.Username.ToLower() == cleanUsername.ToLower());
                if (existing != null)
                {
                    return new RegisterResult { Success = false, Message = "Tên đăng nhập đã tồn tại trong hệ thống!" };
                }

                int nextId = (db.Staffs.Max(s => (int?)s.StaffId) ?? 0) + 1;
                string staffCode = "NV" + nextId.ToString("D2");

                var newStaff = new Staff
                {
                    StaffCode = staffCode,
                    FullName = fullName.Trim(),
                    Username = cleanUsername,
                    PasswordHash = PasswordHasher.Hash(password),
                    Role = role,
                    IsActive = true
                };

                db.Staffs.Add(newStaff);
                db.SaveChanges();

                return new RegisterResult { Success = true, Message = "Đăng ký tài khoản thành công!", Staff = newStaff };
            }
        }
    }

    public class RegisterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Staff Staff { get; set; }
    }
}
