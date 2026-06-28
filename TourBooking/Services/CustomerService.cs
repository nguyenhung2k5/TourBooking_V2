using System;
using System.Linq;
using TourBooking.Data;
using TourBooking.Models;

namespace TourBooking.Services
{
    public class CustomerService
    {
        public Customer GetCustomerByPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return null;
            using (var db = new AppDbContext())
            {
                var p = phone.Trim();
                return db.Customers.FirstOrDefault(c => c.Phone == p);
            }
        }

        public Customer GetOrCreateCustomer(string fullName, string phone, string email, string nationalId)
        {
            using (var db = new AppDbContext())
            {
                var p = phone.Trim();
                var customer = db.Customers.FirstOrDefault(c => c.Phone == p);
                if (customer == null)
                {
                    customer = new Customer
                    {
                        FullName = fullName.Trim(),
                        Phone = p,
                        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                        NationalId = string.IsNullOrWhiteSpace(nationalId) ? null : nationalId.Trim(),
                        CreatedAt = DateTime.Now
                    };
                    db.Customers.Add(customer);
                }
                else
                {
                    customer.FullName = fullName.Trim();
                    if (!string.IsNullOrWhiteSpace(email)) customer.Email = email.Trim();
                    if (!string.IsNullOrWhiteSpace(nationalId)) customer.NationalId = nationalId.Trim();
                }

                db.SaveChanges();
                return customer;
            }
        }
    }
}
