using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using TourBooking.Models;

namespace TourBooking.Services
{
    public class PdfService
    {
        public void GenerateInvoice(Booking booking, string filePath)
        {
            if (booking == null) throw new ArgumentNullException(nameof(booking));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Đường dẫn file không hợp lệ", nameof(filePath));

            Document document = new Document(PageSize.A4, 50, 50, 50, 50);
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // Font chữ chuẩn (Dùng Helvetica mặc định của iTextSharp vì nó luôn có sẵn)
                    Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, BaseColor.DARK_GRAY);
                    Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
                    Font textFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE);

                    // Tiêu đề hóa đơn
                    Paragraph title = new Paragraph("HOA DON THANH TOAN (INVOICE)", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 30;
                    document.Add(title);

                    // Thông tin hóa đơn
                    Paragraph info = new Paragraph();
                    info.Add(new Chunk($"Ma hoa don (Booking Code): {booking.BookingCode}\n", textFont));
                    info.Add(new Chunk($"Ngay dat (Date): {booking.BookingDate:dd/MM/yyyy HH:mm}\n", textFont));
                    info.Add(new Chunk($"Trang thai (Status): {booking.Status}\n", textFont));
                    info.SpacingAfter = 20;
                    document.Add(info);

                    // Thông tin khách hàng
                    Paragraph clientInfo = new Paragraph("THONG TIN KHACH HANG (CUSTOMER INFO)\n", sectionFont);
                    clientInfo.SpacingAfter = 5;
                    document.Add(clientInfo);

                    Paragraph clientDetails = new Paragraph();
                    clientDetails.Add(new Chunk($"Khach hang (Name): {booking.Customer?.FullName}\n", textFont));
                    clientDetails.Add(new Chunk($"So dien thoai (Phone): {booking.Customer?.Phone}\n", textFont));
                    clientDetails.Add(new Chunk($"Email: {booking.Customer?.Email ?? "N/A"}\n", textFont));
                    clientDetails.SpacingAfter = 20;
                    document.Add(clientDetails);

                    // Chi tiết Tour
                    Paragraph tourHeader = new Paragraph("CHI TIET TOUR (TOUR DETAILS)\n", sectionFont);
                    tourHeader.SpacingAfter = 5;
                    document.Add(tourHeader);

                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 4f, 1f, 2f });

                    // Headers
                    PdfPCell cell1 = new PdfPCell(new Phrase("Ten dich vu (Service Name)", headerFont)) { BackgroundColor = BaseColor.BLUE };
                    PdfPCell cell2 = new PdfPCell(new Phrase("SL (Qty)", headerFont)) { BackgroundColor = BaseColor.BLUE, HorizontalAlignment = Element.ALIGN_CENTER };
                    PdfPCell cell3 = new PdfPCell(new Phrase("Thanh tien (Amount)", headerFont)) { BackgroundColor = BaseColor.BLUE, HorizontalAlignment = Element.ALIGN_RIGHT };

                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    table.AddCell(cell3);

                    // Dòng dữ liệu
                    table.AddCell(new PdfPCell(new Phrase(booking.Tour?.TourName ?? "Tour Booking", textFont)));
                    table.AddCell(new PdfPCell(new Phrase("1", textFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase($"{booking.TotalAmount:N0} VND", textFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                    document.Add(table);

                    // Tổng cộng
                    Paragraph total = new Paragraph($"\nTONG CONG (TOTAL): {booking.TotalAmount:N0} VND", sectionFont);
                    total.Alignment = Element.ALIGN_RIGHT;
                    document.Add(total);
                }
            }
            finally
            {
                if (document.IsOpen())
                {
                    document.Close();
                }
            }
        }
    }
}
