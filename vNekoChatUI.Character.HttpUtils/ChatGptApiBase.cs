using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

//ChatGPT API����ʱ��Ҫ�õ��Ķ���
namespace vNekoChatUI.Character.HttpUtils
{
    /// <summary>
    /// �۸���F���ޤ���
    /// </summary>
    internal enum Roles
    {
        /// <summary>
        /// �����ƥࡣAI ���h�����뤨�ޤ���
        /// </summary>
        System,

        /// <summary>
        /// ��`���`��
        /// </summary>
        User,

        /// <summary>
        /// ����������ȡ������ AI �Ǥ���
        /// </summary>
        Assistant
    }

    /// <summary>
    /// ChatGPT �ˤ������å��`�����F���ޤ���
    /// </summary>
    /// <param name="Role"> ��å��`���������ߤ��۸��ȡ�ä��ޤ��� </param>
    /// <param name="Content"> ��å��`���α��Ĥ�ȡ�ä��ޤ��� </param>
    internal record ChatGPTMessage([property: JsonPropertyName("role")] Roles Role,
                                   [property: JsonPropertyName("content")] string Content);

    /// <summary>
    /// ???
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="FinishReason"></param>
    /// <param name="Index"></param>
    internal record ChatGPTChoice([property: JsonPropertyName("message")] ChatGPTMessage Message,
                                  [property: JsonPropertyName("finish_reason")] string FinishReason,
                                  [property: JsonPropertyName("index")] int Index);

    /// <summary>
    /// ChatGPT ��ʹ���Ͻ������F���ޤ���
    /// </summary>
    /// <param name="PromptTokens">��`���`�����������ץ��ץȤΥȩ`��������</param>
    /// <param name="CompletionTokens">AI �����ɤ�����å��`���Υȩ`��������</param>
    /// <param name="TotalTokens">��Ӌ�Υȩ`������������˻��Ť����n�𤵤�ޤ���</param>
    internal record ChatGPTUsage([property: JsonPropertyName("prompt_tokens")] int PromptTokens,
                                 [property: JsonPropertyName("completion_tokens")] int CompletionTokens,
                                 [property: JsonPropertyName("total_tokens")] int TotalTokens);


    /// <summary>
    /// ChatGPT�Υ쥹�ݥ󥹤��F���ޤ���
    /// </summary>
    /// <param name="Id">�쥹�ݥ󥹤� ID��</param>
    /// <param name="ObjectName">�쥹�ݥ󥹤����ɤ����h������ǰ��</param>
    /// <param name="CreatedTimeStamp">�쥹�ݥ󥹤����ɕr�̡�</param>
    /// <param name="ModelName">ʹ�ä�����ǥ롣</param>
    /// <param name="Usage">ʹ���Ͻ�����</param>
    /// <param name="Choices">AI �����ɤ������κ��a��</param>
    internal record ChatGPTResponse()
    {
        /// <summary>
        /// �쥹�ݥ󥹤� ID��
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// �쥹�ݥ󥹤����ɤ����h������ǰ��
        /// </summary>
        [JsonPropertyName("object")]
        public string? ObjectName { get; set; }

        /// <summary>
        /// �쥹�ݥ󥹤����ɕr�̡�
        /// </summary>
        [JsonPropertyName("created")]
        public long? CreatedTimeStamp { get; set; }

        /// <summary>
        /// ʹ�ä�����ǥ롣
        /// </summary>
        [JsonPropertyName("model")]
        public string? ModelName { get; set; }

        /// <summary>
        /// ʹ���Ͻ�����
        /// </summary>
        [JsonPropertyName("usage")]
        public ChatGPTUsage? Usage { get; set; }

        /// <summary>
        /// AI �����ɤ������κ��a��
        /// </summary>
        [JsonPropertyName("choices")]
        public ChatGPTChoice[]? Choices { get; set; }




        /// <summary>
        /// �ֶ�����Flag��true������falseͨ��ʧ��
        /// </summary>
        [JsonIgnore]
        public bool Flag = false;
    };

    /// <summary>
    ///  �����ʽ
    /// </summary>
    internal class ChatGPTRequest
    {
        public ChatGPTRequest(string model, IEnumerable<ChatGPTMessage> messages)
        {
            Model = model;
            Messages = messages;
        }

        /// <summary>
        /// ʹ�ä����ǥ�� ID ��ȡ�ä��ޤ���
        /// Chat API �Ǆ��������ǥ��Ԕ���ˤĤ��Ƥϡ���ǥ� ����ɥݥ���Ȥλ��Q�ԥƩ`�֥����դ��Ƥ���������
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; }

        /// <summary>
        /// ����åȤ��a������ɤ����å��`����ȡ�ä��ޤ���
        /// </summary>
        [JsonPropertyName("messages")]
        public IEnumerable<ChatGPTMessage> Messages { get; }

        /// <summary>
        /// ʹ�ä��륵��ץ���¶Ȥ�ȡ�ä��ޤ���
        /// �ǥե���Ȥ� 1 �ǡ��Є��ʹ���� 0 ���� 2 �Ǥ���0.8 �Τ褦�ʸߤ����ϳ������������ˤ��ޤ�����0.2 �Τ褦�ʵͤ����Ϥ�꼯�еĤǴ_���Ĥʤ�Τˤ��ޤ���
        /// </summary>
        [JsonPropertyName("temprature")]
        public double? Temperature { get; set; } = null;

        /// <summary>
        /// �˥���ץ�󥰤�ȡ�ä��ޤ���
        /// �¶Ȥˤ�륵��ץ�󥰤δ����ֶΤǤ��ꡢ��ǥ�� top_p �_���|����֤ĥȩ`����νY���򿼑]���ޤ����������äơ�0.1 �ϡ���λ 10% �δ_���|���򘋳ɤ���ȩ`����Τߤ����]����뤳�Ȥ���ζ���ޤ���
        /// <see cref="Temperature"/> �I���������뤳�ȤϤ���ᤷ�ޤ���
        /// </summary>
        [JsonPropertyName("top_p")]
        public double? TopP { get; set; } = null;

        /// <summary>
        /// ������å��`�����Ȥ����ɤ������å����ˤ��x�k֫������ȡ�ä��ޤ���
        /// �ǥե���Ȥ� 1 �Ǥ���
        /// </summary>
        [JsonPropertyName("n")]
        public int? N { get; set; } = null;

        /// <summary>
        /// �����򥹥ȥ�`�ह�뤫�������ȡ�ä��ޤ���
        /// �O������ȡ�ChatGPT �Τ褦�˲��ֵĤʥ�å��`�� �ǥ륿�����Ť���ޤ����ȩ`����ϡ����ÿ��ܤˤʤ�ȥǩ`���ΤߤΥ��`�Щ`���ť��٥�ȤȤ������Ť��졢���ȥ�`���data: [DONE]��å��`���ǽK�ˤ��ޤ���
        /// �ǥե���Ȥ� <c>false</c> �Ǥ���
        /// </summary>
        [JsonPropertyName("stream")]
        public bool? DoStreamimg { get; set; } = null;

        /// <summary>
        /// API ���������ϤΥȩ`��������ɤ�ֹͣ������� 4 �ĤΥ��`���󥹤�ȡ�ä��ޤ���
        /// �ǥե���Ȥ� <c>null</c> �Ǥ���
        /// </summary>
        [JsonPropertyName("stop")]
        public string[]? Stop { get; set; } = null;

        /// <summary>
        /// ����å����ˤ����ɤ���ȩ`������������ȡ�ä��ޤ���
        /// �����ȩ`��������ɤ��줿�ȩ`����κ�Ӌ���L���ϡ���ǥ�Υ���ƥ����Ȥ��L���ˤ�ä����ޤ���ޤ���
        /// �ǥե���Ȥ� +INF �Ǥ���
        /// </summary>
        [JsonPropertyName("max_tokens")]
        public int? MaxTokenCount { get; set; } = null;

        /// <summary>
        /// �ץ쥼��_�ڥʥ�ƥ� �΂���ȡ�ä��ޤ���
        /// -2.0 ���� 2.0 �ޤǤ����������΂��ϡ�����ޤǤΥƥ����Ȥ˳��F���뤫�ɤ����˻��Ť����¤����ȩ`����˥ڥʥ�ƥ����n������ǥ뤬�¤����ȥԥå��ˤĤ���Ԓ�������Ԥ�ߤ�ޤ���
        /// </summary>
        [JsonPropertyName("presence_penalty")]
        public double? PresencePenalty { get; set; } = null;

        /// <summary>
        /// �l��_�ڥʥ�ƥ� �΂���ȡ�ä��ޤ���
        /// -2.0 ���� 2.0 �ޤǤ����������΂��ϡ�����ޤǤΥƥ������ڤμȴ���l�Ȥ˻��Ť����¤����ȩ`����˥ڥʥ�ƥ����n������ǥ뤬ͬ���Ф����Z�Ĥ��R�귵�������Ԥ�p�餷�ޤ���
        /// </summary>
        [JsonPropertyName("frequency_penalty")]
        public double? FrequencyPenalty { get; set; } = null;

        /// <summary>
        /// ָ�������ȩ`�����a��˱�ʾ���������Ԥ������낎��ȡ�ä��ޤ���
        /// �ȩ`����(�ȩ`���ʥ����`�Υȩ`���� ID ��ָ��) �� -100 ���� 100 ���v�B����Х��������˥ޥåפ��� json ���֥������Ȥ��ܤ����ޤ�����ѧ�Ĥˤϡ�����ץ�󥰤�ǰ�˥�ǥ�ˤ�ä����ɤ��줿���åȤ˥Х�������׷�Ӥ���ޤ������_�ʄ����ϥ�ǥ뤴�Ȥˮ��ʤ�ޤ�����-1 ���� 1 ���g�΂��Ǥϡ��x�k�ο����Ԥ��p�٤ޤ��ω��Ӥ��ޤ���-100 �� 100 �ʤɤ΂���ָ������ȡ��v�B����ȩ`���󤬽�ֹ�ޤ��������Ĥ��x�k����ޤ���
        /// </summary>
        [JsonPropertyName("logit_bias")]
        public Dictionary<string, double>? LogitBias { get; set; } = null;

        /// <summary>
        /// ����ɥ�`���`���һ����R�e�Ӥ�ȡ�ä��ޤ���
        /// ����ϡ�OpenAI �������О��Oҕ����ӗʳ�����Τ��������ޤ���
        /// </summary>
        [JsonPropertyName("user")]
        public string? User { get; set; } = null;
    }
}

// ���ϲ��õ������¼ʱ�õ������ݸ�ʽ
namespace vNekoChatUI.Character.HttpUtils
{
    //internal class InputData
    //{
    //    [JsonPropertyName("ai_name")]
    //    public string Ai_Name { get; set; } = string.Empty;

    //    [JsonPropertyName("ai_profile")]
    //    public string Ai_Profile { get; set; } = string.Empty;

    //    [JsonPropertyName("ai_content")]
    //    public List<Ai_Content> Ai_Content { get; set; } = new();
    //}

    //internal class Ai_Content
    //{
    //    [JsonPropertyName("roles")]
    //    public string Roles { get; set; } = string.Empty;

    //    [JsonPropertyName("content")]
    //    public string Content { get; set; } = string.Empty;
    //}
}

//���ظ��ϲ�����ݸ�ʽ
namespace vNekoChatUI.Character.HttpUtils
{
    public class Ai_Response
    {
        [JsonPropertyName("totaltokens")]
        public int TotalTokens { get; set; } = int.MinValue;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        public string GetJsonStr()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });
        }
    }
}
