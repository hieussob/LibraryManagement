using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace LibraryManagement.Services
{
    public class QRCodeService
    {
        public string GenerateQRCode(string text)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new QRCode(qrCodeData);
                using var qrCodeImage = qrCode.GetGraphic(20);
                
                using var ms = new MemoryStream();
                qrCodeImage.Save(ms, ImageFormat.Png);
                var bytes = ms.ToArray();
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating QR code: {ex.Message}");
                return string.Empty;
            }
        }

        public Bitmap? GetQRCodeBitmap(string base64String)
        {
            try
            {
                if (string.IsNullOrEmpty(base64String))
                    return null;

                var bytes = Convert.FromBase64String(base64String);
                using var ms = new MemoryStream(bytes);
                return new Bitmap(ms);
            }
            catch
            {
                return null;
            }
        }

        public bool ExportQRCodeToFile(string base64String, string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(base64String))
                    return false;

                var bytes = Convert.FromBase64String(base64String);
                File.WriteAllBytes(filePath, bytes);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting QR code: {ex.Message}");
                return false;
            }
        }
    }
}
