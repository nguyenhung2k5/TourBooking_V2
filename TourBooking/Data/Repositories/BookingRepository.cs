using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TourBooking.Models;
using TourBooking.Data.Interfaces;

namespace TourBooking.Data.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<Booking> GetBookingsByStaff(int staffId)
        {
            return _dbSet.Include(b => b.Tour)
                         .Include(b => b.Customer)
                         .Where(b => b.StaffId == staffId)
                         .ToList();
        }

        public IEnumerable<Booking> GetAllWithDetails()
        {
            return _dbSet.Include(b => b.Tour)
                         .Include(b => b.Customer)
                         .Include(b => b.Staff)
                         .ToList();
        }

        public IEnumerable<Booking> GetByStatus(BookingStatus status)
        {
            return _dbSet.Include(b => b.Tour)
                         .Include(b => b.Customer)
                         .Where(b => b.Status == status)
                         .ToList();
        }
    }
}