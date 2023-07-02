using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using vNekoChatUI.Character.SocketUtils.Extensions;
using vNekoChatUI.Character.SocketUtils.Service;

namespace vNekoChatUI.Character.SocketUtils
{
    // 非同期処理でソケット情報を保持する為のオブジェクト
    public class StateObject
    {
        // 受信バッファサイズ
        public const int BufferSize = 1024;

        // 受信バッファ
        public byte[] buffer = new byte[BufferSize];

        // 受信データ
        public StringBuilder sb = new StringBuilder();

        // ソケット
        public Socket workSocket = null;

        // 服务端专用，储存客户端对应的key
        //public string key = string.Empty;
    }


    //作为客户端，向服务端报道时，的信件格式
    public record ClientInitialAuthentication([property: JsonPropertyName("Name")] string Name,
                                              [property: JsonPropertyName("Age")] string Age);


    //发消息，转发消息，收消息，使用通用格式
    public record ClientMessage([property: JsonPropertyName("SenderName")] string SenderName,
                                [property: JsonPropertyName("ReceiverName")] string ReceiverName,
                                [property: JsonPropertyName("Message")] string Message);
}

namespace vNekoChatUI.Character.SocketUtils
{
    public class ChatBase
    {
        protected string CharacterName { get; init; }
        protected string CharacterAge { get; init; }

        private readonly IJsonService _jsonService;
        //private readonly IQueueService _queueService;

        public ChatBase()
        {
            //注入一个json服务
            _jsonService = new ServiceCollection().TryAddJsonService()
                                                  .BuildServiceProvider()
                                                  .GetRequiredService<IJsonService>();

            //_queueService = new ServiceCollection().TryAddQueueService()
            //                                       .BuildServiceProvider()
            //                                       .GetRequiredService<IQueueService>();
        }


        //序列化
        protected string JsonSerialize<T>(T jsonObject)
        {
            return _jsonService.JsonSerialize(jsonObject);
        }
        //反序列化
        protected T? JsonDeserialize<T>(string jsonText)
        {
            return _jsonService.JsonDeserialize<T>(jsonText);
        }



        ////服务端转发消息时，塞进队列来防止混乱
        //protected void EnqueueTask(Action<Action> action)
        //{
        //    _queueService.EnqueueTask(action);
        //}
    }
}
