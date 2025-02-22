namespace Common.WebWpfCommon
{
    /// <summary>
    /// SignalR Hub 所有方法名
    /// </summary>
    public class ChatHubMethods
    {
        /// <summary>
        /// RegistersServerId (<see cref="string"/> serverId)
        /// </summary>
        public const string RegistersServerId = nameof(RegistersServerId);

        /// <summary>
        /// <para>SendPrivateMessage (<see cref="byte"/>[] method_content, <see cref="string"/>? clientId = null)</para>
        /// <para>若 clientId 为 null ，则消息发送至【通过 RegistersServerId 方法注册为 ChatCenter】 的客户端</para>
        /// </summary>
        public const string SendPrivateMessage = nameof(SendPrivateMessage);

        /// <summary>
        /// <para>SendPublicMessage(<see cref="byte"/>[] method_content)</para>
        /// <para>群发消息，一般不使用</para>
        /// </summary>
        public const string SendPublicMessage = nameof(SendPublicMessage);
    }

    /// <summary>
    /// ChatClient 所有方法名
    /// </summary>
    public class ChatClientMethods
    {
        public const string PushChatHistory = nameof(PushChatHistory);
        public const string StartStreaming = nameof(StartStreaming);
        public const string PushStep = nameof(PushStep);

        public static List<string> MethodList =
            [
            PushChatHistory,
            StartStreaming,
            PushStep,
            ];
    }

    /// <summary>
    /// ChatCenter 所有方法名
    /// </summary>
    public class ChatCenterMethods
    {
        public const string ClearChatHistory = nameof(ClearChatHistory);
        public const string GetChatHistory = nameof(GetChatHistory);
        public const string SendMessage = nameof(SendMessage);
        public const string SendStop = nameof(SendStop);
        public const string GetDefaultProfile = nameof(GetDefaultProfile);
        public const string SetWebPageContext = nameof(SetWebPageContext);

        public static List<string> MethodList =
            [
            ClearChatHistory,
            GetChatHistory,
            SendMessage,
            SendStop,
            GetDefaultProfile,
            SetWebPageContext,
            ];
    }
}
