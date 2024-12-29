using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Interfaces.UIComponents;
using TrarsUI.Shared.Messages;

namespace vNekoChatUI.MVVM.ViewModels
{
    partial class MainWindowVM : ObservableObject, IMainWindowVM
    {
        public ObservableCollection<IToken> SubViewModelList { get; init; }
        public ObservableCollection<string> DialogMessageList { get; init; }

        [ObservableProperty]
        private TaskCompletionSource<bool> taskCompletionSource;

        public MainWindowVM(IMessageBoxService messageBox,
                            ITokenProviderService tokenProvider,
                            IContentProviderService contentProvider,
                            IAbstractFactory<IChildForm> childFormFactory,
                            IAbstractFactory<IuTitleBarVM> titleBarFactory,
                            IAbstractFactory<IuRainbowLineVM> rainbowLineFactory,
                            IAbstractFactory<IuClientVM> clientFactory)
        {
            var token = tokenProvider.MainWindowToken;

            var titleBar = titleBarFactory.Create();
            var rainbowLine = rainbowLineFactory.Create();
            var client = clientFactory.Create();

            titleBar.Token = rainbowLine.Token = client.Token = token;

            this.DialogMessageList = new();
            this.SubViewModelList = new()
            {
                titleBar,
                rainbowLine,
                client,
            };

            // 设置内容
            {
                var contentVM = contentProvider.Create();
                {
                    titleBar.Title = $"{contentVM?.Title ?? "null"}"; // + " -> " + token;
                }
                client.SetContent(contentVM);
            }

            // 打开子窗体
            WeakReferenceMessenger.Default.Register<OpenChildFormMessage>(this, (r, m) =>
            {
                var childForm = childFormFactory.Create();
                {
                    var content = m.Value;
                    childForm.SetWindowInfo(content.WindowInfo);
                    childForm.SetClientContent(content.ViewModel);
                    childForm.SetTitleBarIcon(content.Icon);
                    childForm.Show();
                }
            });

            // 本地弹框
            WeakReferenceMessenger.Default.Register<DialogYesNoMessage, string>(this, token, (r, m) =>
            {
                m.Reply(((Func<Task<bool>>)(() =>
                {
                    m.Callback?.Invoke(this.DialogMessageList.Clear);
                    this.DialogMessageList.Add(m.Message);
                    TaskCompletionSource = new TaskCompletionSource<bool>();
                    return TaskCompletionSource.Task;
                }))());
            });
        }
    }
}
