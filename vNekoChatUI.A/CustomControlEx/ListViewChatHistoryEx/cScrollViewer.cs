using Common.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using vNekoChatUI.Base.Helper.Extensions;

namespace vNekoChatUI.A.CustomControlEx.ListViewChatHistoryEx
{
    public partial class ScrollInfoAdapter : UIElement, IScrollInfo
    {
        private IScrollInfo _child;
        private double _computedVerticalOffset = 0;
        private double _computedHorizontalOffset = 0;
        private const double _scrollLineDelta = 16.0;
        //internal const double _mouseWheelDelta = 48.0;
        private double _mouseWheelDelta => ViewportHeight * 0.25;

        private double _scrollableHeight = 0.0;//记录当前可滚动高度
        //private bool _isItemChanged => (_child.ScrollOwner.ScrollableHeight != _scrollableHeight);//判断可滚动高度是否变化
        private bool _isHeightIncrease => _child.ScrollOwner.ScrollableHeight > _scrollableHeight;//只靠高度判断有点顶不顺，编辑的时候换个行就触发了- -
        private bool _isHeightDecrease => _child.ScrollOwner.ScrollableHeight < _scrollableHeight;
        private int _itemCount = 0;
        private bool _timerLock = false;

        public ScrollInfoAdapter(IScrollInfo child,
                                 Action? ScrollChangedCallback = null,
                                 Func<int>? ListViewItemChangedCallback = null)
        {
            _child = child;

            _child.ScrollOwner.ScrollChanged += (s, e) =>
            {
                var item_count = ListViewItemChangedCallback?.Invoke();
                {
                    //LogProxy.Instance.Print($"{_child.ScrollOwner.ScrollableHeight},{_scrollableHeight}");

                    if (item_count != _itemCount) { Timer(); }
                    if (_isHeightIncrease && (_timerLock))
                    {
                        //LogProxy.Instance.Print($"{_child.ScrollOwner.VerticalOffset} to {_computedVerticalOffset + _child.ScrollOwner.ScrollableHeight}");
                        //增加，用动画调整Offset滚到底
                        VerticalScroll(_computedVerticalOffset + _child.ScrollOwner.ScrollableHeight);
                    }
                    else if (_isHeightDecrease && (_timerLock))
                    {
                        //减少，直接设置Offset
                        _computedVerticalOffset = _child.ScrollOwner.VerticalOffset;
                        //LogProxy.Instance.Print($"_computedVerticalOffset 设置成 {_computedVerticalOffset}");
                    }
                    _scrollableHeight = _child.ScrollOwner.ScrollableHeight;
                    ScrollChangedCallback?.Invoke();
                }
                _itemCount = item_count ?? 0;
            };
        }
        private void Timer()
        {
            //_timerLock = true;
            //return;
            if (_timerLock is false)
            {
                _timerLock = true;
                Task.Run(async () =>
                {
                    await Task.Delay(500);
                    //意为500毫秒之后，即使_isHeightIncrease为true，也不允许执行滚到底动画
                    //使得添加新Item导致的高度变动可以触发完整的滚到底动画（500ms内）
                    //使得修改文本时换行导致的高度变动不会触发滚到底动画（500ms外）
                    //将就用，这样足够了
                    _timerLock = false;
                });
            }
        }
    }
    public partial class ScrollInfoAdapter
    {
        public bool CanVerticallyScroll
        {
            get => _child.CanVerticallyScroll;
            set => _child.CanVerticallyScroll = value;
        }
        public bool CanHorizontallyScroll
        {
            get => _child.CanHorizontallyScroll;
            set => _child.CanHorizontallyScroll = value;
        }

        public double ExtentWidth => _child.ExtentWidth;

        public double ExtentHeight => _child.ExtentHeight;

        public double ViewportWidth => _child.ViewportWidth;

        public double ViewportHeight => _child.ViewportHeight;

        public double HorizontalOffset => _child.HorizontalOffset;
        public double VerticalOffset => _child.VerticalOffset;

        public ScrollViewer ScrollOwner
        {
            get => _child.ScrollOwner;
            set => _child.ScrollOwner = value;
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return _child.MakeVisible(visual, rectangle);
        }

        #region normal
        public void LineUp()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.LineUp();
            else
                VerticalScroll(_computedVerticalOffset - _scrollLineDelta);
        }

        public void LineDown()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.LineDown();
            else
                VerticalScroll(_computedVerticalOffset + _scrollLineDelta);
        }

        public void LineLeft()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.LineLeft();
            else
                HorizontalScroll(_computedHorizontalOffset - _scrollLineDelta);
        }

        public void LineRight()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.LineRight();
            else
                HorizontalScroll(_computedHorizontalOffset + _scrollLineDelta);
        }
        //
        public void MouseWheelUp()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.MouseWheelUp();
            else
            {
                //LogProxy.Instance.Print($"UP {_computedVerticalOffset}, {VerticalScrollOffset}, {_child.ScrollOwner.VerticalOffset}");
                if (_computedVerticalOffset == VerticalScrollOffset && _computedVerticalOffset == 0)
                {
                    _computedVerticalOffset = VerticalScrollOffset = _child.ScrollOwner.VerticalOffset;
                }
                VerticalScroll(_computedVerticalOffset - _mouseWheelDelta);
            }


        }
        //
        public void MouseWheelDown()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.MouseWheelDown();
            else
            {
                //LogProxy.Instance.Print($"down {_computedVerticalOffset}, {VerticalScrollOffset}, {_child.ScrollOwner.VerticalOffset}");
                if (_computedVerticalOffset == VerticalScrollOffset && _computedVerticalOffset == 0)
                {
                    _computedVerticalOffset = VerticalScrollOffset = _child.ScrollOwner.VerticalOffset;
                }
                VerticalScroll(_computedVerticalOffset + _mouseWheelDelta);
            }
        }

        public void MouseWheelLeft()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.MouseWheelLeft();
            else
                HorizontalScroll(_computedHorizontalOffset - _mouseWheelDelta);
        }

        public void MouseWheelRight()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.MouseWheelRight();
            else
                HorizontalScroll(_computedHorizontalOffset + _mouseWheelDelta);
        }

        public void PageUp()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.PageUp();
            else
                VerticalScroll(_computedVerticalOffset - ViewportHeight);
        }

        public void PageDown()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.PageDown();
            else
                VerticalScroll(_computedVerticalOffset + ViewportHeight);
        }

        public void PageLeft()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.PageLeft();
            else
                HorizontalScroll(_computedHorizontalOffset - ViewportWidth);
        }

        public void PageRight()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.PageRight();
            else
                HorizontalScroll(_computedHorizontalOffset + ViewportWidth);
        }

        public void SetHorizontalOffset(double offset)
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.SetHorizontalOffset(offset);
            else
            {
                _computedHorizontalOffset = offset;
                Animate(HorizontalScrollOffsetProperty, offset, 0);
            }
        }

        public void SetVerticalOffset(double offset)
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.SetVerticalOffset(offset);
            else
            {
                _computedVerticalOffset = offset;
                Animate(VerticalScrollOffsetProperty, offset, 0);
            }
        }
        #endregion

        #region not exposed methods
        private void Animate(DependencyProperty property, double targetValue, int duration = 300)
        {
            //make a smooth animation that starts and ends slowly
            var keyFramesAnimation = new DoubleAnimationUsingKeyFrames();
            keyFramesAnimation.Duration = TimeSpan.FromMilliseconds(duration);
            keyFramesAnimation.KeyFrames.Add(
                new SplineDoubleKeyFrame(
                    targetValue,
                    KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(duration)),
                    new KeySpline(0.5, 0.0, 0.5, 1.0)
                    )
                );
            BeginAnimation(property, keyFramesAnimation);
        }

        private void VerticalScroll(double val)
        {
            if (Math.Abs(_computedVerticalOffset - ValidateVerticalOffset(val)) > 0.1)//prevent restart of animation in case of frequent event fire
            {
                _computedVerticalOffset = ValidateVerticalOffset(val);
                Animate(VerticalScrollOffsetProperty, _computedVerticalOffset);
                //LogProxy.Instance.Print($"{VerticalScrollOffset} -> {_computedVerticalOffset}");
            }
        }

        private void HorizontalScroll(double val)
        {
            if (Math.Abs(_computedHorizontalOffset - ValidateHorizontalOffset(val)) > 0.1)//prevent restart of animation in case of frequent event fire
            {
                _computedHorizontalOffset = ValidateHorizontalOffset(val);
                Animate(HorizontalScrollOffsetProperty, _computedHorizontalOffset);
            }
        }

        private double ValidateVerticalOffset(double verticalOffset)
        {
            if (verticalOffset < 0)
                return 0;
            if (verticalOffset > _child.ScrollOwner.ScrollableHeight)
                return _child.ScrollOwner.ScrollableHeight;
            return verticalOffset;
        }

        private double ValidateHorizontalOffset(double horizontalOffset)
        {
            if (horizontalOffset < 0)
                return 0;
            if (horizontalOffset > _child.ScrollOwner.ScrollableWidth)
                return _child.ScrollOwner.ScrollableWidth;
            return horizontalOffset;
        }
        #endregion

        #region helper dependency properties as scrollbars are not animatable by default
        internal double VerticalScrollOffset
        {
            get { return (double)GetValue(VerticalScrollOffsetProperty); }
            set { SetValue(VerticalScrollOffsetProperty, value); }
        }
        internal static readonly DependencyProperty VerticalScrollOffsetProperty =
            DependencyProperty.Register("VerticalScrollOffset", typeof(double), typeof(ScrollInfoAdapter),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnVerticalScrollOffsetChanged)));
        private static void OnVerticalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var smoothScrollViewer = (ScrollInfoAdapter)d;
            if (e.NewValue is not double.NaN)
            {
                smoothScrollViewer._child.SetVerticalOffset((double)e.NewValue);
            }
        }

        internal double HorizontalScrollOffset
        {
            get { return (double)GetValue(HorizontalScrollOffsetProperty); }
            set { SetValue(HorizontalScrollOffsetProperty, value); }
        }
        internal static readonly DependencyProperty HorizontalScrollOffsetProperty =
            DependencyProperty.Register("HorizontalScrollOffset", typeof(double), typeof(ScrollInfoAdapter),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnHorizontalScrollOffsetChanged)));
        private static void OnHorizontalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var smoothScrollViewer = (ScrollInfoAdapter)d;
            smoothScrollViewer._child.SetHorizontalOffset((double)e.NewValue);
        }
        #endregion
    }


    public partial class cScrollViewer : ScrollViewer
    {
        static cScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cScrollViewer), new FrameworkPropertyMetadata(typeof(cScrollViewer)));
        }

        public cScrollViewer()
        {
            //this.CanContentScroll = false;

            Loaded += (s, e) =>
            {
                this.ScrollInfo = new ScrollInfoAdapter(this.ScrollInfo, OnScrollChanged,
                                                                         OnListViewItemChanged);
            };
        }


        //截图用
        private FormatConvertedBitmap FrameworkElementToBitmapSource(FrameworkElement element, double last = 0)
        {
            var presentationSource = PresentationSource.FromVisual(element);
            double mx;// = presentationSource.CompositionTarget.TransformToDevice.M11;
            double my;//= presentationSource.CompositionTarget.TransformToDevice.M22;

            try
            {
                mx = presentationSource.CompositionTarget.TransformToDevice.M11;
                my = presentationSource.CompositionTarget.TransformToDevice.M22;
            }
            catch (Exception ex)
            {
                mx = my = 1.0;
                System.Windows.MessageBox.Show($"{ex.Message}");
            }

            var dpiX = 96.0 * mx;
            var dpiY = 96.0 * my;
            var width = element.ActualWidth * mx;
            var height = element.ActualHeight * my;

            element.UpdateLayout();
            var rtb = new RenderTargetBitmap((int)width, (int)height, dpiX, dpiY, PixelFormats.Pbgra32);
            {
                //在此操作背景色会造成麻烦，故改为在opencvsharp中上背景色
            }
            rtb.Render(element);

            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            if (last == 0)
            {
                newFormatedBitmapSource.BeginInit();
                newFormatedBitmapSource.Source = rtb;
                newFormatedBitmapSource.DestinationFormat = PixelFormats.Bgra32;
                newFormatedBitmapSource.EndInit();
            }
            else
            {
                last *= my;
                newFormatedBitmapSource.BeginInit();
                newFormatedBitmapSource.Source = new CroppedBitmap(rtb, new Int32Rect(0, (int)(height - last), (int)width, (int)last));
                newFormatedBitmapSource.DestinationFormat = PixelFormats.Bgra32;
                newFormatedBitmapSource.EndInit();
            }

            return newFormatedBitmapSource;
        }
        private void BitmapSourceToPngFile(BitmapSource bitmapSource, string pngFilePath)
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
        public void CaptureScreenshot()
        {
            if (this.AllowScreenshot)
            {
                if (IsCapturingScreenshot is false)
                {
                    IsCapturingScreenshot = true;
                    Task.Run(async () =>
                    {
                        LogProxy.Instance.Print($"11111111111");
                        try
                        {
                            //抵消切换BUG带来的"滚动条滚不动"影响
                            {
                                this.Dispatcher.Invoke(() => { this.ScrollToVerticalOffset(1); });
                                await Task.Delay(50);//给点反应时间
                                this.Dispatcher.Invoke(() => { this.ScrollToVerticalOffset(0); });
                                await Task.Delay(100);//给点反应时间
                            }

                            var step = this.ViewportHeight;
                            var count = this.ScrollableHeight / step;//共计要截这么多张图
                            var list = new List<FormatConvertedBitmap>();
                            {
                                LogProxy.Instance.Print($"22222222222222");

                                //正常处理部分
                                for (int i = 0; i < (int)count + 1; i++)
                                {
                                    //正式开始截图
                                    await Task.Delay(50);
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        this.ScrollToVerticalOffset(i * step);
                                    });

                                    await Task.Delay(50);
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        var bs = FrameworkElementToBitmapSource(this);
                                        list.Add(bs);
                                        //BitmapSourceToPngFile(bs, $@"D:\BitmapSource{i}.png");
                                    });
                                }

                                //最后一截不完整处理部分
                                {
                                    var last = 0.0;

                                    await Task.Delay(50);
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        last = this.ScrollableHeight - step * (int)count;
                                        if (last > 0)
                                        {
                                            this.ScrollToVerticalOffset(this.ScrollableHeight);//拉到底
                                        }
                                    });

                                    await Task.Delay(50);
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        if (last > 0)
                                        {
                                            var bs = FrameworkElementToBitmapSource(this, last);
                                            list.Add(bs);
                                            //LogProxy.Instance.Print($"剩余部分 {this.ScrollableHeight} - {step * (int)count} = {last}");
                                            //BitmapSourceToPngFile(bs, @"D:\BitmapSource_last.png");
                                        }
                                    });
                                }

                                this.Dispatcher.Invoke(() =>
                                {
                                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                                    string filePath = Path.Combine(desktopPath, "_messages.png");
                                    var result = Base.Helper.OpenCvProxy.OpenCV.Instance.MergeImages(list, "#FF444654");
                                    BitmapSourceToPngFile(result, filePath);

                                    LogProxy.Instance.Print($"已截图");
                                    IsCapturingScreenshot = false;
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"{ex.Message}");
                        }
                        finally
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                this.IsCapturingScreenshot = false;
                            });
                        }
                    });
                }
            }
        }


        // 滚动条变化
        private void OnScrollChanged()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                var flag1 = ((this.ExtentHeight > this.ViewportHeight) && (this.VerticalOffset > 0));
                if (HasHiddenItemsAbove != flag1) { HasHiddenItemsAbove = flag1; }//触顶

                var flag2 = ((this.ExtentHeight > this.ViewportHeight) && !(this.VerticalOffset + this.ViewportHeight == this.ExtentHeight));
                if (HasHiddenItemsBelow != flag2) { HasHiddenItemsBelow = flag2; }//触底 
            });
        }
        // 成员数量变化
        private int OnListViewItemChanged()
        {
            var obj = this.FindVisualAncestor<ListView>();
            return obj?.Items?.Count ?? 0;
        }
    }
    public partial class cScrollViewer
    {
        public bool HasHiddenItemsAbove
        {
            get { return (bool)GetValue(HasHiddenItemsAboveProperty); }
            set { SetValue(HasHiddenItemsAboveProperty, value); }
        }
        public static readonly DependencyProperty HasHiddenItemsAboveProperty = DependencyProperty.Register(
            name: "HasHiddenItemsAbove",
            propertyType: typeof(bool),
            ownerType: typeof(cScrollViewer),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public bool HasHiddenItemsBelow
        {
            get { return (bool)GetValue(HasHiddenItemsBelowProperty); }
            set { SetValue(HasHiddenItemsBelowProperty, value); }
        }
        public static readonly DependencyProperty HasHiddenItemsBelowProperty = DependencyProperty.Register(
            name: "HasHiddenItemsBelow",
            propertyType: typeof(bool),
            ownerType: typeof(cScrollViewer),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public bool AllowScreenshot
        {
            get { return (bool)GetValue(AllowScreenshotProperty); }
            set { SetValue(AllowScreenshotProperty, value); }
        }
        public static readonly DependencyProperty AllowScreenshotProperty = DependencyProperty.Register(
            name: "AllowScreenshot",
            propertyType: typeof(bool),
            ownerType: typeof(cScrollViewer),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public bool IsCapturingScreenshot
        {
            get { return (bool)GetValue(IsCapturingScreenshotProperty); }
            set { SetValue(IsCapturingScreenshotProperty, value); }
        }
        public static readonly DependencyProperty IsCapturingScreenshotProperty = DependencyProperty.Register(
            name: "IsCapturingScreenshot",
            propertyType: typeof(bool),
            ownerType: typeof(cScrollViewer),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
