using System.Collections.Generic;

namespace vNekoChatUI.Character.BingUtils.Models
{
    internal class NetworkRequest
    {
        public string invocationId { get; set; }
        public string target { get; set; } = "chat";
        public int type { get; set; } = 4;
        public List<NetworkRequestArgument> arguments { get; set; } = new();
    }

    internal class NetworkRequestArgument
    {
        public string source { get; set; } = "cib";

        public string conversationId { get; set; }

        public List<string> optionsSets { get; set; } = new();

        public List<string> allowedMessageTypes { get; set; } = new();

        public List<string> sliceIds { get; set; } = new();

        public string verbosity { get; set; } = "verbose";

        public bool isStartOfSession { get; set; }

        public NetworkMessage Message { get; set; }

        public ParticipantModel participant { get; set; }

        public string conversationSignature { get; set; }


        public List<previousMessage> previousMessages { get; set; }
    }
}
