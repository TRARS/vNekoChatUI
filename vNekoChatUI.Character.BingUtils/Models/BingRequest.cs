using Common.WebWpfCommon;

namespace vNekoChatUI.Character.BingUtils.Models;

public record BingRequest(string user_say)
{
    public ConversationSession Session { get; set; }
    public InputData InputData { get; set; }
}

public record ConversationSession(int InvocationId, string ConversationId, string ClientId, string Signature, string? EncryptedSignature);
