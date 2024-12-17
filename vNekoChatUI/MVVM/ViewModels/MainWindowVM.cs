using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Interfaces.UIComponents;
using TrarsUI.Shared.Messages;

namespace vNekoChatUI.MVVM.ViewModels
{
    partial class MainWindowVM : IMainWindowVM
    {
        public ObservableCollection<IToken> SubViewModelList { get; init; }

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
                    childForm.SetClientContent(content.ViewModel);
                    childForm.SetTitleBarIcon(content.Icon);
                    childForm.Show();
                }
            });
        }
    }
}
