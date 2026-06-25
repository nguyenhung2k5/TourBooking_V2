using System.Linq;
using TourBooking.Models;
using TourBooking.Data.Interfaces;

namespace TourBooking.Data.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }

        public Customer FindByPhone(string phone)
        {
            return _dbSet.FirstOrDefault(c => c.Phone == phone);
        }

        public Customer GetCustomerWithHistory(int id)
        {
            return _dbSet.FirstOrDefault(c => c.CustomerId == id);
        }
    }
}