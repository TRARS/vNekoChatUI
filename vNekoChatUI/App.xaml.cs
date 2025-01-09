using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using TrarsUI.Shared.Helper.Extensions;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Interfaces.UIComponents;
using TrarsUI.Shared.Services;
using vNekoChatUI.A.Factories;
using vNekoChatUI.MVVM.ViewModels;
using vNekoChatUI.MVVM.Views;

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
            AppHost.Services.GetRequiredService<IAbstractFactory<IMainWindow>>().Create().Show();
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
                           // Service
                           sc.AddSingleton<IMessageBoxService, MessageBoxService>();
                           sc.AddScoped<ITokenProviderService, TokenProviderService>();
                           sc.AddSingleton<IContentProviderService, AContentProviderService>();
                           sc.AddTransient<IDebouncerService, DebouncerService>();
                           // UI组件VM
                           sc.AddFormFactory<IuTitleBarVM, uTitleBarVM>();
                           sc.AddFormFactory<IuRainbowLineVM, uRainbowLineVM>();
                           sc.AddFormFactory<IuClientVM, uClientVM>();

                           // MainWindow MainWindowVM
                           sc.AddFormFactory<IMainWindow, IMainWindowEmpty, MainWindow>(sp =>
                           {
                               using (var scope = sp.CreateScope())
                               {
                                   var mainwindow = (MainWindow)(scope.ServiceProvider.GetRequiredService<IMainWindowEmpty>());
                                   {
                                       mainwindow.DataContext = scope.ServiceProvider.GetRequiredService<IMainWindowVM>();
                                       //mainwindow.SizeToContent = SizeToContent.WidthAndHeight;
                                       mainwindow.ResizeMode = ResizeMode.CanResize;
                                       mainwindow.Width = 660;
                                       mainwindow.Height = 480;
                                       mainwindow.MinWidth = 660;
                                       mainwindow.MinHeight = 480;
                                       mainwindow.MaxWidth = 960;
                                       mainwindow.MaxHeight = 720;
                                   }
                                   return mainwindow;
                               }
                           });
                           sc.AddScoped<IMainWindowVM, MainWindowVM>();

                           // ChildForm ChildFormVM
                           sc.AddFormFactory<IChildForm, IChildFormEmpty, ChildForm>(sp =>
                           {
                               using (var scope = sp.CreateScope())
                               {
                                   var childForm = (ChildForm)scope.ServiceProvider.GetRequiredService<IChildFormEmpty>();
                                   {
                                       childForm.DataContext = scope.ServiceProvider.GetRequiredService<IChildFormVM>();
                                       childForm.SizeToContent = SizeToContent.WidthAndHeight;
                                   }
                                   return childForm;
                               }
                           });
                           sc.AddScoped<IChildFormVM, ChildFormVM>();
                       });
        }

        public static T GetRequiredService<T>() where T : notnull
        {
            return AppHost.Services.GetRequiredService<T>();
        }
    }
}
