using System;
using System.Security.Cryptography;
using System.Text;

namespace TourBooking.Helpers
{
    public static class PasswordHasher
    {
        // Hàm băm mật khẩu thô sang chuỗi SHA256 để đối soát với DB
        public static string Hash(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}