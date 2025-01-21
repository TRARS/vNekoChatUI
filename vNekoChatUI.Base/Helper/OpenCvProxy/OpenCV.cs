using Common.WPF;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace vNekoChatUI.Base.Helper.OpenCvProxy
{
    /// <summary>
    /// OpenCvSharp封装
    /// </summary>
    public partial class OpenCV
    {
        private static readonly Lazy<OpenCV> lazyObject = new(() => new OpenCV());
        public static OpenCV Instance => lazyObject.Value;
    }

    //对外公开方法
    public partial class OpenCV
    {
        /// <summary>
        /// <para>bmp: 位图</para>
        /// <para>savePath: 绝对路径 @"X:\img\output.png"</para>
        /// </summary>
        public void SaveBitmapSource(BitmapSource bmp, string savePath)
        {
            try
            {
                Cv2.ImWrite(savePath, OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(bmp));
            }
            catch (Exception ex) { LogProxy.Instance.Print($"{ex.Message}"); }
        }

        /// <summary>
        /// <para>bmpList: 等宽位图List</para>
        /// <para>bkColor: 背景色 "#AARRGGBB"</para>
        /// </summary>
        public BitmapSource? MergeImages(List<FormatConvertedBitmap> bmpList, string bkColor)
        {
            try
            {
                List<Mat> matList = new();
                bmpList.ForEach((item) =>
                {
                    matList.Add(OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(item));
                });
                return MergeImagesBase(matList, bkColor);
            }
            catch (Exception ex) { LogProxy.Instance.Print($"{ex.Message}"); }
            return null;
        }

        /// <summary>
        /// <para>bmpList: 等宽位图List</para>
        /// <para>bkColor: 背景色 "#AARRGGBB"</para>
        /// <para>savePath: 绝对路径 @"X:\img\output.png"</para>
        /// </summary>
        public void MergeImagesAnsSaveToImg(List<FormatConvertedBitmap> bmpList, string bkColor, string savePath)
        {
            try
            {
                List<Mat> matList = new();
                bmpList.ForEach((item) =>
                {
                    matList.Add(OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(item));
                });

                Cv2.ImWrite(savePath, OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(MergeImagesBase(matList, bkColor)));
            }
            catch (Exception ex) { LogProxy.Instance.Print($"{ex.Message}"); }
        }

        /// <summary>
        /// <para>亮度转不透明度</para>
        /// <para>bmp: 位图</para>
        /// <para>startValue:  起点（比如填"64"，灰度小于64的像素变透明）</para>
        /// <para>endValue:  终点（比如填"192"，灰度大于192的像素变透明）</para>
        /// </summary>
        public BitmapSource? ExtractOutline(string bmpPath, byte startValue = 0, byte endValue = 255)
        {
            try
            {
                Mat mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(new System.Drawing.Bitmap(bmpPath));
                return ExtractOutlineBase(mat, startValue, endValue);
            }
            catch (Exception ex) { LogProxy.Instance.Print($"{ex.Message}"); }
            return null;
        }
    }

}

namespace vNekoChatUI.Base.Helper.OpenCvProxy
{
    //需求实现
    public partial class OpenCV
    {
        //垂直合并图片
        private BitmapSource MergeImagesBase(List<Mat> _matList, string _bkcolor)
        {
            using (Mat flowMat = new())
            {
                _matList.ForEach((item) => { SaveToFlow(item, flowMat); });

                {
                    var color = (Color)ColorConverter.ConvertFromString(_bkcolor);// "#FF46567A";//CV_8UC4
                    using (Mat backgroundMat = new Mat(flowMat.Height, flowMat.Width, flowMat.Type(), new Scalar(color.B, color.G, color.R, color.A)))
                    {
                        //Mat result = new();
                        ////切割bgra通道
                        //Cv2.Split(backgroundMat, out var bk_bgra);
                        //Cv2.Split(flowMat, out var fl_bgra);

                        ////背景bgr_a
                        //Mat backgroundMat_bgr = new();
                        //Mat backgroundMat_a = new();
                        //Cv2.Merge(new Mat[] { bk_bgra[0], bk_bgra[1], bk_bgra[2] }, backgroundMat_bgr);
                        //Cv2.Merge(new Mat[] { bk_bgra[3], bk_bgra[3], bk_bgra[3] }, backgroundMat_a);

                        ////前景bgr_a
                        //Mat flowMat_bgr = new();
                        //Mat flowMat_a = new();
                        //Cv2.Merge(new Mat[] { fl_bgra[0], fl_bgra[1], fl_bgra[2] }, flowMat_bgr);
                        //Cv2.Merge(new Mat[] { fl_bgra[3], fl_bgra[3], fl_bgra[3] }, flowMat_a);

                        //result = backgroundMat_bgr.BitwiseAnd(~flowMat_a).ToMat()
                        //                          .BitwiseOr(flowMat_bgr);
                        //↑有问题，全是狗牙

                        //↓到头来还是遍历
                        unsafe
                        {
                            byte* fptr = (byte*)flowMat.Ptr(0).ToPointer();//foreground
                            byte* bptr = (byte*)backgroundMat.Ptr(0).ToPointer();//background
                            for (int i = 0; i < (flowMat.Width * flowMat.Height * 4); i += 4) //4通道
                            {
                                byte a = fptr[i + 3];//前景色不透明度捞出来拿去计算
                                bptr[i + 0] = Blend(bptr[i + 0], fptr[i + 0], a);
                                bptr[i + 1] = Blend(bptr[i + 1], fptr[i + 1], a);
                                bptr[i + 2] = Blend(bptr[i + 2], fptr[i + 2], a);
                                bptr[i + 3] = Blend_Alpha(bptr[i + 3], a);
                            }
                        }

                        return OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(backgroundMat.Clone());
                    }
                }
            }
        }

        //亮度转不透明度，提取线稿
        private BitmapSource ExtractOutlineBase(Mat _mat, byte startValue, byte endValue)
        {
            using (Mat srcMat = _mat.CvtColor(ColorConversionCodes.BGR2BGRA))
            {
                unsafe
                {
                    byte* sptr = (byte*)srcMat.Ptr(0).ToPointer();
                    for (int i = 0; i < (srcMat.Width * srcMat.Height * 4); i += 4) //4通道
                    {
                        if (sptr[i + 3] == 0) { continue; }

                        byte gray = (byte)Math.Clamp(0.2989 * sptr[i + 2] + 0.5870 * sptr[i + 1] + 0.1140 * sptr[i + 0], byte.MinValue, byte.MaxValue);
                        if (gray < startValue || gray > endValue)
                        {
                            sptr[i + 3] = 0;
                        }
                        else
                        {
                            sptr[i + 0] = sptr[i + 1] = sptr[i + 2] = 0;
                            sptr[i + 3] = MapRange(gray, startValue, endValue);
                        }
                    }

                    return OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(srcMat.Clone());
                }
            }
        }
    }

    public partial class OpenCV
    {
        //将流程储存至单个Mat以便展示
        private void SaveToFlow(Mat input, Mat flow)
        {
            if (flow.Width == 0 || flow.Height == 0)
            {
                Cv2.CopyTo(input, flow);
            }
            else
            {
                Cv2.VConcat(flow, input, flow);
            }
        }

        private byte Blend(byte _a, byte _b, double _r)
        {
            double a = _a / 255.0;
            double b = _b / 255.0;
            double r = _r / 255.0;

            return (byte)Math.Clamp((a * (1 - r) + b * r) * byte.MaxValue, byte.MinValue, byte.MaxValue);
        }
        private byte Blend_Alpha(byte _a, byte _b)
        {
            double a = _a / 255.0;
            double b = _b / 255.0;

            return (byte)Math.Clamp((a + b - a * b) * byte.MaxValue, byte.MinValue, byte.MaxValue);
        }

        private byte MapRange(byte _value, byte _startValue, byte _endValue)
        {
            if (_startValue >= _endValue) { return _value; }

            // 确保输入值在起点和终点范围内
            if (_value < _startValue)
                _value = _startValue;
            else if (_value > _endValue)
                _value = _endValue;

            // 计算映射后的值
            byte mappedValue = (byte)Math.Clamp(255 - (((_value - _startValue) / (double)(_endValue - _startValue)) * 255), byte.MinValue, byte.MaxValue);
            if (mappedValue / 4 < 8) { mappedValue = 0; }
            else { mappedValue = (byte)Math.Clamp(mappedValue * 1.1, 0, 255); }//加黑
            return mappedValue;
        }
    }
}
