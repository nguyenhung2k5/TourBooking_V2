using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using TourBooking.Data;

namespace TourBooking.Services
{
    public class ReportService
    {
        public bool ExportReportToFile(string reportType, string targetFilePath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                using (var context = new AppDbContext())
                {
                    switch (reportType)
                    {
                        case "Tour":
                            sb.AppendLine("Ma Tour,Ten Tour,Diem Den,Ngay Khoi Hanh,Ngay Ve,Gia Nguoi Lon,Gia Tre Em,Tong Cho,Cho Trong,Trang Thai");
                            var tours = context.Tours.ToList();
                            foreach (var t in tours)
                            {
                                string status = t.IsActive ? "Hoat dong" : "Tam dung";
                                sb.AppendLine($"\"{t.TourCode}\",\"{t.TourName}\",\"{t.Destination}\",\"{t.DepartureDate:dd/MM/yyyy}\",\"{t.ReturnDate:dd/MM/yyyy}\",{t.PriceAdult},{t.PriceChild},{t.TotalSlots},{t.AvailableSlots},\"{status}\"");
                            }
                            break;

                        case "Booking":
                            sb.AppendLine("Ma Don,Khach Hang,Tour,Ngay Dat,Tong Tien,Trang Thai");
                            var bookings = context.Bookings.Include(b => b.Customer).Include(b => b.Tour).ToList();
                            foreach (var b in bookings)
                            {
                                string custName = b.Customer != null ? b.Customer.FullName : "N/A";
                                string tourName = b.Tour != null ? b.Tour.TourName : "N/A";
                                sb.AppendLine($"\"{b.BookingCode}\",\"{custName}\",\"{tourName}\",\"{b.BookingDate:dd/MM/yyyy HH:mm}\",{b.TotalAmount},\"{b.Status}\"");
                            }
                            break;

                        case "Staff":
                            sb.AppendLine("Ma Nhan Vien,Ho Ten,Ten Dang Nhap,Vai Tro,Trang Thai");
                            var staffs = context.Staffs.ToList();
                            foreach (var s in staffs)
                            {
                                string status = s.IsActive ? "Hoat dong" : "Khoa";
                                sb.AppendLine($"\"{s.StaffCode}\",\"{s.FullName}\",\"{s.Username}\",\"{s.Role}\",\"{status}\"");
                            }
                            break;

                        case "Customer":
                            sb.AppendLine("ID Khach Hang,Ho Ten,So Dien Thoai,Email,CMND/CCCD,Ngay Tao");
                            var customers = context.Customers.ToList();
                            foreach (var c in customers)
                            {
                                sb.AppendLine($"{c.CustomerId},\"{c.FullName}\",\"{c.Phone}\",\"{c.Email}\",\"{c.NationalId}\",\"{c.CreatedAt:dd/MM/yyyy}\"");
                            }
                            break;
                    }
                }

                File.WriteAllText(targetFilePath, sb.ToString(), Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
