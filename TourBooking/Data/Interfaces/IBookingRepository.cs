using System.Collections.Generic;
using TourBooking.Models;

namespace TourBooking.Data.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        IEnumerable<Booking> GetBookingsByStaff(int staffId);
        IEnumerable<Booking> GetAllWithDetails();
        IEnumerable<Booking> GetByStatus(BookingStatus status);
    }
}