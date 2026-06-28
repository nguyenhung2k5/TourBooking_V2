using System;
using System.IO;
using System.Windows.Media.Imaging;
using QRCoder;

namespace TourBooking.Services
{
    public class QrService
    {
        public BitmapImage GeneratePaymentQr(string bookingCode, decimal amount)
        {
            try
            {
                // Thông tin chuyển khoản giả lập (Ví dụ VietQR: danh sách tham số qua link ngân hàng)
                // Định dạng VietQR: https://img.vietqr.io/image/<BANK_ID>-<ACCOUNT_NO>-<TEMPLATE>.png?amount=<AMOUNT>&addInfo=<DESCRIPTION>
                string paymentInfo = $"Ngân hàng: VietinBank\nSố TK: 101888888888\nTên TK: CONG TY NET TOUR\nSố tiền: {amount:N0} VND\nNội dung: {bookingCode}";
                
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(paymentInfo, QRCodeGenerator.ECCLevel.Q))
                    {
                        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                        {
                            byte[] qrBytes = qrCode.GetGraphic(20);
                            
                            BitmapImage bitmap = new BitmapImage();
                            using (MemoryStream ms = new MemoryStream(qrBytes))
                            {
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = ms;
                                bitmap.EndInit();
                            }
                            bitmap.Freeze(); // Cần thiết để sử dụng trên Thread UI của WPF
                            return bitmap;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
