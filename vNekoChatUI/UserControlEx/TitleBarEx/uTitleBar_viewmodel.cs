using vNekoChatUI.Base.Helper;

namespace vNekoChatUI.UserControlEx.TitleBarEx
{
    //构造函数
    public partial class uTitleBar_viewmodel : NotificationObject
    {
        private uTitleBar_model model = new();

        public uTitleBar_viewmodel()
        {
            this.Title = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} ({System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location):yyyy-MM-dd HH:mm:ss})";
        }
    }

    //self_model
    public partial class uTitleBar_viewmodel
    {
        public string Title
        {
            get { return model.Title; }
            set
            {
                if (model.Title == value)
                    return;
                model.Title = value;
                NotifyPropertyChanged();
            }
        }
    }
}
