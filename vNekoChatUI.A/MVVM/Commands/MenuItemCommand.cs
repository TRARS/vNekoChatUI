using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace vNekoChatUI.A.MVVM.Commands
{
    //附加小按钮
    public partial class MenuItemCommand : ObservableObject
    {
        public string Header { get; set; }
        public string HeaderStringFormat { get; set; }
        public Action<object> OnClicked { get; set; }

        [RelayCommand]
        private void OnClick(object para)
        {
            OnClicked?.Invoke(para);
        }
    }
}
