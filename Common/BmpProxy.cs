using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Common
{
    //单例
    public partial class BmpProxy
    {
        private static readonly object objlock = new object();
        private static BmpProxy? _instance;
        public static BmpProxy Instance
        {
            get
            {
                lock (objlock)
                {
                    if (_instance is null)
                    {
                        _instance = new BmpProxy();
                    }
                }
                return _instance;
            }
        }
    }

    public partial class BmpProxy
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
