namespace TourBooking.Models
{
    public enum BookingStatus { Pending, Paid, Cancelled, Refunded }
    public enum PaymentMethod { Cash, Transfer }
    public enum UserRole { Staff, Admin }
    public enum PassengerType { Adult, Child }
}