using System.Collections.Generic;
using TourBooking.Models;

namespace TourBooking.Data.Interfaces
{
    public interface IStaffRepository : IGenericRepository<Staff>
    {
        Staff Authenticate(string username, string passwordHash);
        IEnumerable<Staff> GetActiveStaff();
    }
}