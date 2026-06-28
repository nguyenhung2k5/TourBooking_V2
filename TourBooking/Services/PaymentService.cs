using System;
using System.Linq;
using TourBooking.Data;
using TourBooking.Models;

namespace TourBooking.Services
{
    public class PaymentService
    {
        public bool ProcessPayment(int bookingId, decimal amount, PaymentMethod method)
        {
            using (var db = new AppDbContext())
            {
                var booking = db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
                if (booking == null) return false;

                var payment = new Payment
                {
                    BookingId = bookingId,
                    Amount = amount,
                    Method = method,
                    PaidAt = DateTime.Now
                };

                db.Payments.Add(payment);
                booking.Status = BookingStatus.Paid;
                db.SaveChanges();
                return true;
            }
        }
    }
}
