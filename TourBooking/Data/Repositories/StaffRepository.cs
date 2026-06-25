using System.Collections.Generic;
using System.Linq;
using TourBooking.Models;
using TourBooking.Data.Interfaces;

namespace TourBooking.Data.Repositories
{
    public class StaffRepository : GenericRepository<Staff>, IStaffRepository
    {
        public StaffRepository(AppDbContext context) : base(context)
        {
        }

        public Staff Authenticate(string username, string passwordHash)
        {
            return _dbSet.FirstOrDefault(s => s.Username == username
                                           && s.PasswordHash == passwordHash
                                           && s.IsActive);
        }

        public IEnumerable<Staff> GetActiveStaff()
        {
            return _dbSet.Where(s => s.IsActive).ToList();
        }
    }
}