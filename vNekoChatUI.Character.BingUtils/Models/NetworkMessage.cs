using System;

namespace vNekoChatUI.Character.BingUtils.Models;

public class NetworkMessage
{
    public string text { get; set; }

    public string messageType { get; set; } = "Chat";

    public string inputMethod { get; set; } = "Keyboard";

    public string author { get; set; } = "user";

    public DateTimeOffset timestamp { get; set; }

    public string region { get; set; } = "AU";
}

public class ParticipantModel
{
    public string id { get; set; }
}

public class previousMessage
{
    public string author { get; set; }
    public string description { get; set; }
    public string contextType { get; set; }
    public string messageType { get; set; }
    public string messageId { get; set; }
}
