using TourBooking.Models;

namespace TourBooking.Data.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Customer FindByPhone(string phone);
        Customer GetCustomerWithHistory(int id);
    }
}