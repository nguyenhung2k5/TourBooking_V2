using System;
using System.IO;
using System.Text;
using System.Globalization;
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

                    
                    Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, new BaseColor(15, 23, 42)); // Slate-900
                    Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, new BaseColor(30, 41, 59)); // Slate-800
                    Font textFont = FontFactory.GetFont(FontFactory.HELVETICA, 10.5f, new BaseColor(71, 85, 105)); // Slate-600
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10.5f, BaseColor.WHITE);

                    
                    Paragraph title = new Paragraph("HOA DON THANH TOAN (INVOICE)", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 25;
                    document.Add(title);

                    
                    Paragraph info = new Paragraph();
                    info.Add(new Chunk($"Ma hoa don (Booking Code): {RemoveAccents(booking.BookingCode)}\n", textFont));
                    info.Add(new Chunk($"Ngay dat (Date): {booking.BookingDate:dd/MM/yyyy HH:mm}\n", textFont));
                    info.Add(new Chunk($"Trang thai (Status): {RemoveAccents(booking.Status.ToString())}\n", textFont));
                    info.SpacingAfter = 15;
                    document.Add(info);

                    
                    Paragraph clientInfo = new Paragraph("THONG TIN KHACH HANG (CUSTOMER INFO)\n", sectionFont);
                    clientInfo.SpacingAfter = 5;
                    document.Add(clientInfo);

                    Paragraph clientDetails = new Paragraph();
                    clientDetails.Add(new Chunk($"Khach hang (Name): {RemoveAccents(booking.Customer?.FullName)}\n", textFont));
                    clientDetails.Add(new Chunk($"So dien thoai (Phone): {RemoveAccents(booking.Customer?.Phone)}\n", textFont));
                    clientDetails.Add(new Chunk($"Email: {RemoveAccents(booking.Customer?.Email ?? "N/A")}\n", textFont));
                    clientDetails.SpacingAfter = 15;
                    document.Add(clientDetails);

                   
                    Paragraph tourHeader = new Paragraph("CHI TIET TOUR (TOUR DETAILS)\n", sectionFont);
                    tourHeader.SpacingAfter = 5;
                    document.Add(tourHeader);

                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 5f, 1f, 2f });

                    
                    BaseColor headerBgColor = new BaseColor(37, 99, 235); 

                    
                    PdfPCell cell1 = new PdfPCell(new Phrase("Ten dich vu (Service Name)", headerFont)) { BackgroundColor = headerBgColor, Padding = 6 };
                    PdfPCell cell2 = new PdfPCell(new Phrase("SL (Qty)", headerFont)) { BackgroundColor = headerBgColor, HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 };
                    PdfPCell cell3 = new PdfPCell(new Phrase("Thanh tien (Amount)", headerFont)) { BackgroundColor = headerBgColor, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 6 };

                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    table.AddCell(cell3);

                    
                    string tourNameUnsigned = RemoveAccents(booking.Tour?.TourName ?? "Tour Booking");
                    table.AddCell(new PdfPCell(new Phrase(tourNameUnsigned, textFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase("1", textFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{booking.TotalAmount:N0} VND", textFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 6 });

                    document.Add(table);

                   
                    Paragraph total = new Paragraph($"\nTONG CONG (TOTAL): {booking.TotalAmount:N0} VND", sectionFont);
                    total.Alignment = Element.ALIGN_RIGHT;
                    document.Add(total);

                    document.Close();
                }
            }
            catch (Exception)
            {
                if (document.IsOpen())
                {
                    document.Close();
                }
                throw;
            }
        }

        // Hàm chuyển đổi tiếng Việt có dấu thành không dấu
        private string RemoveAccents(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (c == 'đ' || c == 'Đ')
                    {
                        stringBuilder.Append(c == 'đ' ? 'd' : 'D');
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
