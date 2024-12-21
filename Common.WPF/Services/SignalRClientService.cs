using Common.WebWpfCommon;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.WPF.Services
{
    public interface ISignalRClientService
    {
        public Task Connect();
        public Task Disconnect();

        public Task PushChatHistory(string? para = null);
        public Task StartStreaming(string? para = null);
        public Task PushStep(string? para = null);

        bool[] ServerStart { get; set; }
        bool ServerState { get; }

        Action<string>? SendProxy { get; set; }
        Action<string>? StopProxy { get; set; }
        Action<string>? ClearProxy { get; set; }
        Action<string>? RefreshProfile { get; set; }
        Action? Close { get; set; }
        Func<string>? GetChatHistoryProxy { get; set; }
        Action<string>? SetProfileProxy { get; set; }
        Action<string>? SetInnerMonologueProxy { get; set; }
        Action<string>? SetContinuePrompt { get; set; }
        Action<List<Ai_Content>>? SetChatHistoryProxy { get; set; }
        Action<bool>? SetBingBypassDetectionFlagProxy { get; set; }
    }

    //构造
    public partial class SignalRClientService : ISignalRClientService
    {
        HubConnection connection;

        public SignalRClientService()
        {
            //将就用，不考虑超时。
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:14285/chathub")
                .WithAutomaticReconnect()
                .Build();

            connection.Reconnecting += (sender) =>
            {
                LogProxy.Instance.Print("Attempting to reconnect");
                return Task.CompletedTask;
            };

            connection.Reconnected += (sender) =>
            {
                LogProxy.Instance.Print("Reconnected to the server");
                return Task.CompletedTask;
            };

            connection.Closed += (sender) =>
            {
                this.Close?.Invoke();
                LogProxy.Instance.Print("Connection Closed");
                return Task.CompletedTask;
            };
        }
    }

    //方法
    public partial class SignalRClientService
    {
        bool methodRegistered = false;
        //string chatHistoryBackup = string.Empty;

        //在这里注册供服务端调用的方法
        public async Task Connect()
        {
            //客户端注册几个方法供服务端调用
            if (methodRegistered is false)
            {
                //服务端调用客户端方法
                connection.On<byte[]?>("ExecuteClientCommandWithName", async method_content =>
                {
                    var jsonStr = TextCompressor.Instance.DecompressData(method_content);
                    var jsonObj = JsonSerializer.Deserialize<MethodContent>(jsonStr, new JsonSerializerOptions
                    {
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });

                    if (jsonObj is null) { return; }

                    LogProxy.Instance.Print($"—{jsonObj.MethodName} ok");

                    switch (jsonObj.MethodName)
                    {
                        //服务端主动清除历史记录
                        case "ServerClearChatHistory":
                            {
                                DispatcherInvoke(() => { ClearProxy?.Invoke(""); });
                                break;
                            }

                        //服务端主动拿历史记录
                        case "ServerGetChatHistory":
                            {
                                await PushChatHistory(GetChatHistoryProxy?.Invoke());
                                break;
                            }

                        //服务端点了一下Send按钮
                        case "ServerSendMessage":
                            {
                                DispatcherInvoke(() => { SendProxy?.Invoke(jsonObj.MethodPara); });
                                break;
                            }
                        //服务端点了一下Stop按钮
                        case "ServerSendStop":
                            {
                                DispatcherInvoke(() => { StopProxy?.Invoke(jsonObj.MethodPara); });
                                break;
                            }

                        //服务端点了一下刷新人设
                        case "ServerRefreshProfile":
                            {
                                DispatcherInvoke(() => { RefreshProfile?.Invoke(jsonObj.MethodPara); });
                                await PushChatHistory(GetChatHistoryProxy?.Invoke());//刷新人设后直接全部推过去
                                break;
                            }
                        //服务端修改WebPageContext
                        case "ServerSetWebPageContext":
                            {
                                DispatcherInvoke(() =>
                                {
                                    var inputdata = JsonSerializer.Deserialize<InputData>(jsonObj.MethodPara, new JsonSerializerOptions()
                                    {
                                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                        WriteIndented = true
                                    });

                                    //服务端修改人设
                                    SetProfileProxy?.Invoke(inputdata.Ai_Profile);
                                    //服务端修改内心独白
                                    SetInnerMonologueProxy?.Invoke(inputdata.Ai_InnerMonologue);
                                    //服务端修改ContinuePrompt
                                    SetContinuePrompt?.Invoke(inputdata.Ai_ContinuePrompt);
                                    //服务端发消息可能需要过越狱检测
                                    SetBingBypassDetectionFlagProxy?.Invoke(inputdata.Bypass_Detection);
                                    //服务端可能修改了聊天记录
                                    if (inputdata.History_Changed)
                                    {
                                        SetChatHistoryProxy?.Invoke(inputdata.Ai_Content);
                                    }

                                });

                                break;
                            }

                        //未知
                        default:
                            {
                                LogProxy.Instance.Print($"unknown method: {jsonObj.MethodName}");
                                break;
                            }
                    }
                });

                methodRegistered = true;//防止重复注册
            }

            //尝试连接
            try
            {
                if (connection.State is HubConnectionState.Connected)
                {
                    ServerStart[0] = true;
                    LogProxy.Instance.Print($"Connection Started");
                    return;
                }

                await connection.StartAsync();
                ServerStart[0] = true;
                LogProxy.Instance.Print($"Connection Started");
            }
            catch (Exception ex)
            {
                ServerStart[0] = false;
                LogProxy.Instance.Print($"{ex.Message}");
                if (ex.InnerException != null)
                {
                    LogProxy.Instance.Print($"—— {ex.InnerException.Message}");
                }
            }
        }
        public async Task Disconnect()
        {
            if (connection.State is not HubConnectionState.Disconnected)
            {
                connection?.StopAsync();
                ServerStart[0] = false;
                //LogProxy.Instance.Print($"Connection Closed");
            }
            await Task.Yield();
        }

        //下列方法均对应服务端的"Client + XXXYYYZZZ"方法
        public async Task PushChatHistory(string? para = null)
        {
            try
            {
                if (ServerStart[0] && (connection.State is HubConnectionState.Disconnected))
                {
                    await this.Connect();
                }

                //执行服务端的ClientPushChatHistory方法
                await ExecuteServerCommand(para);
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"{ex.Message}");
            }
        }
        public async Task StartStreaming(string? para = null)
        {
            try
            {
                if (ServerStart[0] && (connection.State is HubConnectionState.Disconnected))
                {
                    await this.Connect();
                }

                //执行服务端的ClientStartStreaming方法
                await ExecuteServerCommand(para ?? "…");

            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"{ex.Message}");
            }
        }
        public async Task PushStep(string? para = null)
        {
            try
            {
                if (ServerStart[0] && (connection.State is HubConnectionState.Disconnected))
                {
                    await this.Connect();
                }

                //执行服务端的ClientPushStep方法
                await ExecuteServerCommand(para);
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"{ex.Message}");
            }
        }
    }

    //JSON
    public partial class SignalRClientService
    {
        //通过[CallerMemberName]特性免去写方法名，只需要服务端存在前缀为"Client"的同名方法即可
        private async Task ExecuteServerCommand(string? methodPara, [CallerMemberName] string? methodName = null)
        {
            if (connection is null || connection.State is not HubConnectionState.Connected) { return; }

            var method_content = new MethodContent
            {
                ConnectionId = connection.ConnectionId,
                MethodName = methodName,
                MethodPara = methodPara,
            };
            var jsonStr = GetMethodContentJsonStr(method_content);
            var data = TextCompressor.Instance.CompressText(jsonStr);//压缩一下！

            await connection.InvokeAsync("ExecuteServerCommandWithName", data);
        }
        //包装一下
        private string GetMethodContentJsonStr(MethodContent content)
        {
            //让服务端解一下json
            var json = JsonSerializer.Serialize(content, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });

            return json;
        }
    }

    //ViewModel那边用
    public partial class SignalRClientService
    {
        public bool[] ServerStart { get; set; } = new bool[1] { false };
        public bool ServerState => connection.State is HubConnectionState.Connected;

        public Action<string>? SendProxy { get; set; }
        public Action<string>? StopProxy { get; set; }
        public Action<string>? ClearProxy { get; set; }
        public Action<string>? RefreshProfile { get; set; }
        public Action? Close { get; set; }
        public Func<string>? GetChatHistoryProxy { get; set; }
        public Action<string>? SetProfileProxy { get; set; }
        public Action<string>? SetInnerMonologueProxy { get; set; }
        public Action<string>? SetContinuePrompt { get; set; }
        public Action<List<Ai_Content>>? SetChatHistoryProxy { get; set; }
        public Action<bool>? SetBingBypassDetectionFlagProxy { get; set; }

        private void DispatcherInvoke(Action act)
        {
            //用Invoke同步
            System.Windows.Application.Current.Dispatcher.Invoke(act);
        }
    }
}
