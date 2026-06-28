using System;
using System.IO;
using System.Text;
using System.Globalization;
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
                
                string bankBin = "970436"; 
                string accountNo = "9356895595"; 

                
                string napasValue = "0006" + bankBin + "01" + accountNo.Length.ToString("D2") + accountNo;
                string subtag01 = "01" + napasValue.Length.ToString("D2") + napasValue;

                
                string subtag00 = "0010A000000727"; 
                string subtag02 = "0208QRIBFTTA";   

                
                string tag38Content = subtag00 + subtag01 + subtag02;
                string tag38 = "38" + tag38Content.Length.ToString("D2") + tag38Content;

               
                string tag53 = "5303704";

                
                string amountStr = ((long)amount).ToString();
                string tag54 = "54" + amountStr.Length.ToString("D2") + amountStr;

                
                string tag58 = "5802VN";

                
                string cleanDesc = RemoveAccents(bookingCode).Replace(" ", "");
                if (cleanDesc.Length > 20) cleanDesc = cleanDesc.Substring(0, 20);
                string tag62Content = "08" + cleanDesc.Length.ToString("D2") + cleanDesc;
                string tag62 = "62" + tag62Content.Length.ToString("D2") + tag62Content;

                
                string rawPayload = "000201" + "010212" + tag38 + tag53 + tag54 + tag58 + tag62 + "6304";

               
                string crc = CalculateCRC16(rawPayload);
                string paymentInfo = rawPayload + crc;

                
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
                            bitmap.Freeze(); 
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

       
        private string CalculateCRC16(string data)
        {
            ushort crc = 0xFFFF;
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            foreach (byte b in bytes)
            {
                crc ^= (ushort)(b << 8);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                    {
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }
            return crc.ToString("X4");
        }

        
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
