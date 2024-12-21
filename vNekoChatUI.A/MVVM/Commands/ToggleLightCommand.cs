using Common.WPF;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace vNekoChatUI.A.MVVM.Commands
{
    //边框发光的包装一下
    public partial class ToggleLightCommand : ObservableObject
    {
        private Action<object> _callback;

        private bool[] _isLightOn;
        public bool IsLightOn
        {
            get { return _isLightOn[0]; }
            set
            {
                SetProperty(ref _isLightOn[0], value);
            }
        }

        public ToggleLightCommand(string propName, bool[] isLightOn, Func<Task>? callback = null)
        {
            _isLightOn = isLightOn;

            _callback = new(async _ =>
            {
                IsLightOn = !IsLightOn;
                if (callback is not null)
                {
                    await callback.Invoke();
                    //IsLightOn = isLightOn[0];//刷新
                    OnPropertyChanged(nameof(IsLightOn));
                }
                LogProxy.Instance.Print($"{propName} = {_isLightOn[0]}");
            });
        }

        [RelayCommand]
        private void OnClick(object para)
        {
            _callback?.Invoke(para);
        }
    }
}
