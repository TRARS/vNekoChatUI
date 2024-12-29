using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;
using System.Windows;
using TrarsUI.Shared.DTOs;
using TrarsUI.Shared.Helper;
using TrarsUI.Shared.Helper.Extensions;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Interfaces.UIComponents;
using TrarsUI.Shared.Messages;
using TrarsUI.Shared.ShadowLayer;
using TrarsUI.SourceGenerator.Attributes;

namespace vNekoChatUI.MVVM.Views
{
    [UseChrome]
    public partial class MainWindow : Window, IMainWindow
    {
        public string Token
        {
            get => token;
            set => token = value;
        }

        string token = string.Empty;
        bool canExit = false;
        bool canDebounce = false;
        ShadowHelper shadowHelper = new();

        public Action? OnTaskbarMinimize { get; set; }

        public MainWindow(ITokenProviderService tokenProvider, IDebouncerService debouncer)
        {
            InitializeComponent();
            InitWindowBorderlessBehavior(); // 无边框

            // 通过任务栏图标最小化
            OnTaskbarMinimize = () =>
            {
                WeakReferenceMessenger.Default.Send(new WindowMinimizeMessage("OnTaskbarMinimize"), token);
            };

            // 设置Token
            token = tokenProvider.MainWindowToken;

            // 标题
            WeakReferenceMessenger.Default.Register<WindowTitleChangedMessage, string>(this, token, (r, m) =>
            {
                this.Title = m.Value;
            });

            // 顶置
            WeakReferenceMessenger.Default.Register<WindowTopmostMessage, string>(this, token, (r, m) =>
            {
                var flag = !((MainWindow)r).Topmost;
                shadowHelper.ShadowTopmost(token, flag);

                ((MainWindow)r).Topmost = flag;
                m.Reply(flag);
            });

            // 重置位置
            WeakReferenceMessenger.Default.Register<WindowPosResetMessage, string>(this, token, async (r, m) =>
            {
                canDebounce = true;
                shadowHelper.ShadowFadeInOut(token, false); await Task.Delay(64);

                ((MainWindow)r).SetDoubleAnimation(OpacityProperty, Opacity, 0d, 96).ContinueWith(async () =>
                {
                    await Task.Delay(1);
                    ((MainWindow)r).TryMoveToPrimaryMonitor(m.Value);
                    await Task.Delay(96);
                    ((MainWindow)r).SetDoubleAnimation(OpacityProperty, Opacity, 1d, 192).ContinueWith(async () =>
                    {
                        shadowHelper.ShadowFadeInOut(token, true);
                        await Task.Delay(128); canDebounce = false;
                        await shadowHelper.ShadowZindex(token);
                    }).Begin();
                }).Begin();
            });

            // 最小化
            WeakReferenceMessenger.Default.Register<WindowMinimizeMessage, string>(this, token, async (r, m) =>
            {
                shadowHelper.ShadowFadeInOut(token, null);
                await Task.Delay(64);
                ((MainWindow)r).WindowState = WindowState.Minimized;
            });

            // 最大化
            WeakReferenceMessenger.Default.Register<WindowMaximizeMessage, string>(this, token, (r, m) =>
            {
                ((MainWindow)r).WindowState = (((MainWindow)r).WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
            });

            // 关闭
            WeakReferenceMessenger.Default.Register<WindowCloseMessage, string>(this, token, (r, m) =>
            {
                canExit = false; shadowHelper.Close(token);

                ((MainWindow)r).SetDoubleAnimation(OpacityProperty, Opacity, 0d, 256).ContinueWith(() =>
                {
                    canExit = true;
                    this.UnRegister();
                    this.Close();
                    Environment.Exit(0);
                }).Begin();
            });

            // 窗体截图
            WeakReferenceMessenger.Default.Register<WindowSaveToTransparentPngMessage, string>(this, token, async (r, m) =>
            {
                await Task.Delay(1000);
                ((MainWindow)r).SaveToPng(allowTransparency: true);
            });

            // Loaded
            this.Loaded += async (s, e) =>
            {
                var stateInfo = new WindowStateInfo(this.WindowState == WindowState.Maximized, this.Topmost);
                WeakReferenceMessenger.Default.Send(new WindowStateUpdateMessage(stateInfo), token);

                // 阴影
                var mainLayer = this;
                var shadowLayer = shadowHelper.CreateSubWindow<Shadow>(shadow =>
                {
                    shadowHelper.ShadowInit(token, shadow, mainLayer, mainLayer.PART_Border.CornerRadius);
                    //shadowHelper.SetOwner(mainLayer, shadow.GetHandle());
                });

                await Task.Delay(512);

                mainLayer.Activate();
            };

            // StateChanged
            this.StateChanged += async (s, e) =>
            {
                var stateInfo = new WindowStateInfo(this.WindowState == WindowState.Maximized, this.Topmost);
                WeakReferenceMessenger.Default.Send(new WindowStateUpdateMessage(stateInfo), token);

                var flag = this.WindowState == WindowState.Normal;
                if (this.WindowState != WindowState.Minimized)
                {
                    shadowHelper.ShadowFadeInOut(token, flag);
                }

                if (this.WindowState == WindowState.Normal)
                {
                    await Task.Delay(128);
                    await shadowHelper.ShadowZindex(token);
                }
            };

            // LocationChanged
            this.LocationChanged += (s, e) =>
            {
                shadowHelper.ShadowMove(token);
            };

            // SizeChanged
            this.SizeChanged += (s, e) =>
            {
                shadowHelper.ShadowMove(token);
            };

            // Activated
            this.Activated += async (s, e) =>
            {
                await shadowHelper.ShadowZindex(token);
                shadowHelper.ShadowBlurRadius(token, true);
                this.IsActive = true;

                debouncer.Cancel();
            };

            // Deactivated
            this.Deactivated += async (s, e) =>
            {
                shadowHelper.ShadowBlurRadius(token, false);

                // 防闪烁
                if (canDebounce)
                {
                    await debouncer.DebounceAsync(async () =>
                    {
                        await Task.Delay(1); this.IsActive = false;
                    }, TimeSpan.FromMilliseconds(192));
                }
                else
                {
                    this.IsActive = false;
                }
            };

            // Closing
            this.Closing += (s, e) =>
            {
                e.Cancel = !canExit;
                WeakReferenceMessenger.Default.Send(new WindowCloseMessage("Closing"), token);
            };
        }
    }
}