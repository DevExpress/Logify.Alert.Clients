using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;

namespace DevExpress.Logify.Core {
    public class ScreenshotCollector : IInfoCollector {
        [HandleProcessCorruptedStateExceptions]
        public virtual void Process(Exception ex, ILogger logger) {
            string content= CreateScreenshotImageBytes();
            if (String.IsNullOrEmpty(content))
                return;

            logger.BeginWriteObject("screenshot");
            try {
                logger.WriteValue("imageBytes", content);
            }
            finally {
                logger.EndWriteObject("screenshot");
            }
        }

        Bitmap CreateScreenshotBitmap() {
            Point location = SystemInformation.VirtualScreen.Location;
            Size size = SystemInformation.VirtualScreen.Size;
            Bitmap result = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(result);
            graphics.CopyFromScreen(location.X, location.Y, 0, 0, size, CopyPixelOperation.SourceCopy);
            return result;
        }
        byte[] GetImageBytes(Bitmap bitmap) {
            using (MemoryStream stream = new MemoryStream()) {
                bitmap.Save(stream, ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }
        string CreateScreenshotImageBytes() {
            try {
                Bitmap bitmap = CreateScreenshotBitmap();
                byte[] bytes = GetImageBytes(bitmap);
                bitmap.Dispose();
                return Convert.ToBase64String(bytes);
            }
            catch {
                return null;
            }
        }
    }
}