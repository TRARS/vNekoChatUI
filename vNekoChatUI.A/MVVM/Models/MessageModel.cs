using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace vNekoChatUI.A.MVVM.Models
{
    internal partial class MessageModel : ObservableObject
    {
        public string Username { get; set; }
        public string UsernameColor { get; set; }
        public string UserborderColor { get; set; }
        public string ImageSource { get; set; }

        public bool IsBot { get; set; }//影响聊天气泡位于左或右

        public string DisplayName { get; set; }

        public bool StartStreaming { get; set; }

        [ObservableProperty]
        private string _message;

        [ObservableProperty]
        private DateTime _time;

        [ObservableProperty]
        private int _tokenPrice;//

        //
        public MessageModel Clone(MessageModel src)
        {
            MessageModel dst = new()
            {
                Username = src.Username,
                UsernameColor = src.UsernameColor,
                UserborderColor = src.UserborderColor,
                ImageSource = src.ImageSource,

                IsBot = src.IsBot,
                DisplayName = src.DisplayName,
                StartStreaming = src.StartStreaming,
                Message = src.Message,
                Time = src.Time,
                TokenPrice = src.TokenPrice
            };

            return dst;
        }
    }
}
