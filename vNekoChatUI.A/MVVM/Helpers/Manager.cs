using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrarsUI.Shared.Helpers.Extensions;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Messages;
using vNekoChatUI.A.MVVM.Models;
using vNekoChatUI.A.MVVM.ViewModels;

namespace vNekoChatUI.A.MVVM.Helpers
{
    internal sealed partial class Manager
    {
        readonly Dictionary<string, string> _windows = new();
        readonly List<string> _tokens = new();

        readonly IAbstractFactory<PEditorVM> _pEditorVMFactory;

        public Manager(IAbstractFactory<PEditorVM> pEditorVMFactory)
        {
            _pEditorVMFactory = pEditorVMFactory;
        }

        public async Task OpenPEditor(ContactModel selectedContact)
        {
            await this.OpenWith<PEditorVM>(async () =>
            {
                var token = await WeakReferenceMessenger.Default.Send(new OpenChildFormMessage(new()
                {
                    Icon = "M0.986196 969.101766L89.250722 598.292135 647.108828 40.434029c25.31236-25.31236 59.500482-39.447833 95.167898-39.447833 35.667416 0 70.019904 14.135474 95.167897 39.447833l132.807704 132.807705c25.31236 25.31236 39.447833 59.500482 39.447834 95.167897 0 35.667416-14.135474 70.019904-39.447834 95.167897L412.394222 921.435634 41.420225 1009.700161c-11.341252 2.629856-23.339968-0.657464-31.558267-8.875763-8.218299-8.382665-11.669984-20.38138-8.875762-31.722632z m633.630818-54.405137h430.803211V1022.35634H634.617014v-107.659711z",
                    ViewModel = _pEditorVMFactory.Create().Init(x => { x.Bot = selectedContact; }),
                    WindowInfo = new()
                    {
                        MinHeight = 608,
                        MaxHeight = 608
                    }
                }));
                return token;
            });
        }
    }

    internal sealed partial class Manager
    {
        private async Task OpenWith<T>(Func<Task<string>> action)
        {
            var type = typeof(T).Name;
            if (this.Begin(type)) { return; }
            var token = await action.Invoke();
            this.After(type, token);
        }

        private bool Begin(string type)
        {
            // 限制打开数量
            if (_windows.ContainsKey(type))
            {
                this.ShowChildForm(_windows[type]); return true;
            }

            return false;
        }

        private void After(string type, string token)
        {
            // VM宿主关闭时反注册
            WeakReferenceMessenger.Default.Register<WindowClosingMessage, string>(this, token, (r, m) =>
            {
                _windows.Remove(type);
                WeakReferenceMessenger.Default.Unregister<WindowClosingMessage, string>(this, token);
            });

            // 内部维护
            _tokens.Add(token);
            _windows.TryAdd(type, token);
        }
    }

    internal sealed partial class Manager
    {
        public void ShowAllChildForm()
        {
            _tokens.ForEach(token =>
            {
                WeakReferenceMessenger.Default.Send(new WindowNormalizeMessage("ShowAllChildForm"), token);
            });
        }

        public void CloseAllChildForm()
        {
            _tokens.ForEach(token =>
            {
                WeakReferenceMessenger.Default.Send(new WindowCloseMessage("CloseAllChildForm"), token);
            });
            _tokens.Clear();
        }

        public void ShowChildForm(string token)
        {
            WeakReferenceMessenger.Default.Send(new WindowNormalizeMessage("ShowChildForm"), token);
        }

        public void CloseChildForm(string token)
        {
            WeakReferenceMessenger.Default.Send(new WindowCloseMessage("CloseAllChildForm"), token);
            _tokens.Remove(token);
        }
    }
}
