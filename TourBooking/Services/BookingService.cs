using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using TourBooking.Data;
using TourBooking.Models;

namespace TourBooking.Services
{
    public class BookingService
    {
        public Booking CreateBooking(int tourId, string customerName, string customerPhone, string customerEmail, string customerNationalId, int staffId, int adultQty, int childQty, decimal totalAmount, PaymentMethod method)
        {
            using (var db = new AppDbContext())
            {
                var tour = db.Tours.FirstOrDefault(t => t.TourId == tourId);
                if (tour == null || !tour.IsActive)
                {
                    throw new InvalidOperationException("Tour không tồn tại hoặc đã ngừng hoạt động.");
                }

                int totalPassengers = adultQty + childQty;
                if (tour.AvailableSlots < totalPassengers)
                {
                    throw new InvalidOperationException("Tour không đủ số chỗ trống.");
                }

                var phone = customerPhone.Trim();
                var customer = db.Customers.FirstOrDefault(c => c.Phone == phone);
                if (customer == null)
                {
                    customer = new Customer
                    {
                        FullName = customerName.Trim(),
                        Phone = phone,
                        Email = string.IsNullOrWhiteSpace(customerEmail) ? null : customerEmail.Trim(),
                        NationalId = string.IsNullOrWhiteSpace(customerNationalId) ? null : customerNationalId.Trim(),
                        CreatedAt = DateTime.Now
                    };
                    db.Customers.Add(customer);
                }
                else
                {
                    customer.FullName = customerName.Trim();
                    customer.Email = string.IsNullOrWhiteSpace(customerEmail) ? customer.Email : customerEmail.Trim();
                    customer.NationalId = string.IsNullOrWhiteSpace(customerNationalId) ? customer.NationalId : customerNationalId.Trim();
                }

                var booking = new Booking
                {
                    BookingCode = "BK" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    TourId = tour.TourId,
                    CustomerId = customer.CustomerId,
                    StaffId = staffId,
                    Status = BookingStatus.Paid,
                    TotalAmount = totalAmount,
                    BookingDate = DateTime.Now
                };

                db.Bookings.Add(booking);
                db.SaveChanges(); 

                if (adultQty > 0)
                {
                    db.BookingDetails.Add(new BookingDetail
                    {
                        BookingId = booking.BookingId,
                        PassengerType = PassengerType.Adult,
                        Quantity = adultQty,
                        UnitPrice = tour.PriceAdult
                    });
                }

                if (childQty > 0)
                {
                    db.BookingDetails.Add(new BookingDetail
                    {
                        BookingId = booking.BookingId,
                        PassengerType = PassengerType.Child,
                        Quantity = childQty,
                        UnitPrice = tour.PriceChild
                    });
                }

                db.Payments.Add(new Payment
                {
                    BookingId = booking.BookingId,
                    Amount = totalAmount,
                    Method = method,
                    PaidAt = DateTime.Now
                });

                tour.AvailableSlots -= totalPassengers;
                db.SaveChanges();

                
                return db.Bookings
                    .Include(b => b.Tour)
                    .Include(b => b.Customer)
                    .Include(b => b.Staff)
                    .FirstOrDefault(b => b.BookingId == booking.BookingId);
            }
        }

        public bool CancelBooking(int bookingId)
        {
            using (var db = new AppDbContext())
            {
                var booking = db.Bookings
                    .Include(b => b.Tour)
                    .Include(b => b.BookingDetails)
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null || booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Refunded)
                {
                    return false;
                }

                booking.Status = BookingStatus.Cancelled;

                if (booking.Tour != null)
                {
                    int totalReturnedSlots = booking.BookingDetails.Sum(d => d.Quantity);
                    booking.Tour.AvailableSlots += totalReturnedSlots;
                }

                db.SaveChanges();
                return true;
            }
        }
    }
}
