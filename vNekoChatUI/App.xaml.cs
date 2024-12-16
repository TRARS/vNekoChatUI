using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using TrarsUI.Shared.Helper.Extensions;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Interfaces.UIComponents;
using TrarsUI.Shared.Services;
using vNekoChatUI.TrarsWindow.MVVM.ViewModels;
using vNekoChatUI.TrarsWindow.MVVM.Views;

namespace vNekoChatUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost AppHost { get; set; } = GetHostBuilder().Build();

        private System.Threading.Mutex mutex = new System.Threading.Mutex(false, $"{Application.ResourceAssembly.GetName().Name} {Environment.UserName}");

        public App()
        {
            // ミューテックスの所有権を要求
            if (!mutex.WaitOne(0, false))
            {
                // 既に起動しているため終了させる
                MessageBox.Show($"{Application.ResourceAssembly.GetName().Name} already running", "Multiple Instances", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                mutex.Close();
                mutex = null;
                this.Shutdown();
            }

            //帧数设置
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(Timeline),
                new FrameworkPropertyMetadata { DefaultValue = null }
            );

            //使SelectionTextBrush生效
            AppContext.SetSwitch("Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering", false);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<MainView.MainWindow>();
            startupForm.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Close();
            }

            await AppHost.StopAsync();
            base.OnExit(e);
        }

        private static IHostBuilder GetHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                       .ConfigureServices(sc =>
                       {
                           sc.AddSingleton<MainView.MainWindow>();

                           // Service
                           sc.AddSingleton<IMessageBoxService, MessageBoxService>();
                           sc.AddSingleton<ITokenProviderService, TokenProviderService>();
                           sc.AddTransient<IDebouncerService, DebouncerService>();
                           // UI组件VM
                           sc.AddFormFactory<IuTitleBarVM, uTitleBarVM>();
                           sc.AddFormFactory<IuRainbowLineVM, uRainbowLineVM>();
                           sc.AddFormFactory<IuClientVM, uClientVM>();
                           // ChildForm ChildFormVM
                           sc.AddFormFactory<IChildForm, IChildFormEmpty, ChildForm>(sp =>
                           {
                               var childForm = (ChildForm)sp.GetRequiredService<IChildFormEmpty>();
                               {
                                   childForm.DataContext = sp.GetRequiredService<IAbstractFactory<IChildFormVM>>().Create();
                                   childForm.SizeToContent = SizeToContent.WidthAndHeight;
                               }
                               return childForm;
                           });
                           sc.AddFormFactory<IChildFormVM, ChildFormVM>();
                       });
        }
    }
}
