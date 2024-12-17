using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Interfaces.UIComponents;
using TrarsUI.Shared.Messages;

namespace vNekoChatUI.MVVM.ViewModels
{
    partial class ChildFormVM : IChildFormVM
    {
        public ObservableCollection<IToken> SubViewModelList { get; init; }

        public ChildFormVM(IMessageBoxService messageBox,
                           ITokenProviderService tokenProvider,
                           IAbstractFactory<IuTitleBarVM> titleBarFactory,
                           IAbstractFactory<IuRainbowLineVM> rainbowLineFactory,
                           IAbstractFactory<IuClientVM> clientFactory)
        {
            var token = tokenProvider.GetRecurrentTokenForChildForm();

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
            WeakReferenceMessenger.Default.Register<SetContentMessage, string>(this, token, (r, m) =>
            {
                var contentVM = m.Value;
                {
                    titleBar.Title = $"{contentVM?.Title ?? "null"}"; // + " -> " + token;
                }
                client.SetContent(contentVM);
            });

            // 设置Icon
            WeakReferenceMessenger.Default.Register<SetTitleBarIconMessage, string>(this, token, (r, m) =>
            {
                titleBar.SetIcon(m.Value);
            });
        }
    }
}
