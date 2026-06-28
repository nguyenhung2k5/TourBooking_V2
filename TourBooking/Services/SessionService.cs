namespace TourBooking.Services
{
    public class SessionService
    {
        private static SessionService _instance;
        public static SessionService Instance => _instance ?? (_instance = new SessionService());

        public class UserSession
        {
            public string FullName { get; set; } = "Nguyễn Trọng Hùng";
            public string Username { get; set; } = "admin";
        }

        public UserSession CurrentStaff { get; set; }

        private SessionService()
        {
            // Khởi tạo phiên giả lập để debug không bị báo lỗi NullReferenceException
            CurrentStaff = new UserSession();
        }

        public void Logout()
        {
            CurrentStaff = null;
        }
    }
}