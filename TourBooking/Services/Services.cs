using TourBooking.Models;

namespace TourBooking.Services
{
    public class SessionService
    {
        private static SessionService _instance;
        public static SessionService Instance => _instance ?? (_instance = new SessionService());

        private SessionService() { }

        // Lưu thông tin nhân viên hoặc admin đang thao tác thực tế
        public static Staff CurrentStaff { get; set; }

        // Kiểm tra xem có phải là Admin hay không dựa trên giá trị Enum quy đổi ra số
        // Trong Enum của bạn: Staff = 0, Admin = 1
        public static bool IsAdmin => CurrentStaff != null && (int)CurrentStaff.Role == 1;

        public static void Logout()
        {
            CurrentStaff = null;
        }
    }
}