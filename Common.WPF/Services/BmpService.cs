using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Common.WPF.Services
{
    public interface IBmpService
    {
        public void BitmapSourceToPngFile(BitmapSource bitmapSource, string pngFilePath);
    }

    public partial class BmpService : IBmpService
    {
        public void BitmapSourceToPngFile(BitmapSource bitmapSource, string pngFilePath)
        {
            try
            {
                using (var stream = new FileStream(pngFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(stream);
                }
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"BitmapSourceToPngFile Error: {ex.Message}");
            }
        }
    }
}
