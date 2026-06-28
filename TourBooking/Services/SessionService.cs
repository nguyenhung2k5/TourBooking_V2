using TourBooking.Models;

namespace TourBooking.Services
{
    public class SessionService
    {
        private static SessionService _instance;
        public static SessionService Instance => _instance ?? (_instance = new SessionService());

        private SessionService() { }

        
        public static Staff CurrentStaff { get; set; }

        
        public static bool IsAdmin => CurrentStaff != null && (int)CurrentStaff.Role == 1;

        public static void Logout()
        {
            CurrentStaff = null;
        }
    }
}
