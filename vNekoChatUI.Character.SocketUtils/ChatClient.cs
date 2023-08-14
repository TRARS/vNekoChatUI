using Common.WPF;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vNekoChatUI.Character.SocketUtils
{
    //客户端
    public partial class ChatClient : ChatBase
    {
        /// <summary>
        /// 构造函数，传入角色名
        /// </summary>
        public ChatClient(string name, int age = 18)
        {
            base.CharacterName = $"{name}";
            base.CharacterAge = $"{age}";
        }
    }
    public partial class ChatClient
    {
        // SemaphoreSlimのインスタンスを生成
        private readonly SemaphoreSlim ewh = new SemaphoreSlim(0, 1);


        // 收到消息时，执行上层提供的回调函数（将 senderName、senderMessage 回传到上层）
        private Action<ClientMessage>? _callback;

        // 储存服务端地址
        private Socket? client_to_server;

        //
        public void StartClient(string ipaddress, int port, Action<ClientMessage>? callback = null)
        {
            // サーバーへ接続
            try
            {
                // IPアドレスとポート番号を取得
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ipaddress), port);

                // TCP/IPのソケットを作成
                Socket? client = new Socket(IPAddress.Parse(ipaddress).AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // サーバーをキャッチ
                client_to_server = client;

                // エンドポイント（IPアドレスとポート）へ接続
                client.BeginConnect(endpoint, new AsyncCallback(InitConnectCallback), client);
                ewh.Wait();  //接続シグナルになるまで待機

                // 初回接続認証（仮）
                string data = JsonSerialize(new ClientInitialAuthentication(base.CharacterName, base.CharacterAge));
                // ASCIIエンコーディングで送信データをバイトの配列に変換
                byte[] byteData = Encoding.Unicode.GetBytes(data + "<EOF>");

                // サーバーへデータを送信
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(InitSendCallback), client);
                ewh.Wait();  //送信シグナルになるまで待機

                // ソケット情報を保持する為のオブジェクトを生成
                StateObject state = new StateObject();
                state.workSocket = client;

                // サーバーからデータ受信
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(InitReceiveCallback), state);
                ewh.Wait();  //受信シグナルになるまで待機

                // ソケット接続終了
                //client.Shutdown(SocketShutdown.Both);
                //client.Close();
                //LogProxy.Instance.Print("接続終了");

                _callback = callback;

                ewh.Dispose();
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"StartClient—{ex.Message}");
            }
        }

        private void InitConnectCallback(IAsyncResult ar)
        {
            try
            {
                // ソケットを取得
                Socket client = (Socket)ar.AsyncState;

                // 非同期接続を終了
                client.EndConnect(ar);
                //LogProxy.Instance.Print("client -> 接続完了");

                // シグナル状態にし、メインスレッドの処理を続行する
                ewh.Release();
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"InitConnectCallback—{ex.Message}");
            }
        }
        private void InitSendCallback(IAsyncResult ar)
        {
            try
            {
                // ソケットを取得
                Socket client = (Socket)ar.AsyncState;

                // 非同期送信を終了
                int bytesSent = client.EndSend(ar);
                //LogProxy.Instance.Print("client -> 送信完了");

                // シグナル状態にし、メインスレッドの処理を続行する
                ewh.Release();
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"InitSendCallback—{ex.Message}");
            }
        }
        private void InitReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // ソケット情報を保持する為のオブジェクトから情報取得
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // 非同期受信を終了
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // 受信したデータを蓄積
                    state.sb.Append(Encoding.Unicode.GetString(state.buffer, 0, bytesRead));

                    // 蓄積データの終端タグを確認
                    var content = state.sb.ToString();

                    // 終了タグ<EOF>があれば、読み取り完了
                    if (content.EndsWith("<EOF>"))
                    {
                        LogProxy.Instance.Print($"☆client({base.CharacterName}) -> Server({client_to_server?.RemoteEndPoint})曰く「{content}」");

                        state.sb.Clear();//需要反复使用该对象，所以清空

                        var jsonObject = this.JsonDeserialize<ClientMessage>(content.TrimEnd("<EOF>".ToCharArray()));
                        if (jsonObject is not null)
                        {
                            _callback?.Invoke(jsonObject);//消息回传给上层
                        }

                        ewh.Release();

                        OnLoginSuccess(client);
                    }
                    else
                    {
                        // 受信処理再開（まだ受信しているデータがあるため）
                        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(InitReceiveCallback), state);
                        //receiveDone.Set();
                    }
                }
                else
                {
                    // ここにまで辿り着けないよう祈る
                    throw new InvalidOperationException("Cliten: The code should not reach this point.");
                }
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"Cliten: InitReceiveCallback—{ex.Message}");
            }
        }
    }


    public partial class ChatClient
    {
        //本客户端已连接到服务端
        private void OnLoginSuccess(Socket client)
        {
            StateObject state = new StateObject();
            state.workSocket = client;
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnServerMessageReceived), state);
        }

        //收到[服务端]消息后，执行该回调函数（具体行为：收消息，返回给上层，清空缓冲区，继续收消息）
        private void OnServerMessageReceived(IAsyncResult ar)
        {
            try
            {
                // ソケット情報を保持する為のオブジェクトから情報取得
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // 非同期受信を終了
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // 受信したデータを蓄積
                    state.sb.Append(Encoding.Unicode.GetString(state.buffer, 0, bytesRead));

                    // 蓄積データの終端タグを確認
                    var content = state.sb.ToString();

                    // 終了タグ<EOF>があれば、読み取り完了
                    if (content.EndsWith("<EOF>"))
                    {
                        LogProxy.Instance.Print($"client({base.CharacterName}) -> Server({client_to_server?.RemoteEndPoint})曰く「{content}」");

                        state.sb.Clear();//bufferをクリアする

                        var jsonObject = this.JsonDeserialize<ClientMessage>(content.TrimEnd("<EOF>".ToCharArray()));
                        if (jsonObject is not null)
                        {
                            _callback?.Invoke(jsonObject);//上の層にメッセージを送り返す
                        }
                    }

                    // 受信処理再開（まだ受信しているデータがあるため）
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnServerMessageReceived), state);
                    //receiveDone.Set();
                }
                else
                {
                    // ここにまで辿り着けないよう祈る
                    throw new InvalidOperationException("Cliten: The code should not reach this point.");
                }
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"Cliten: OnServerMessageReceived—{ex.Message}");
            }
        }

        //服务端主动将消息发送至指定客户端后，执行该回调函数
        private void OnSpecificClientSendCallback(IAsyncResult ar)
        {
            try
            {
                // 非同期オブジェクトからソケット情報を取得
                Socket TcpClient = (Socket)ar.AsyncState;

                // クライアントへデータ送信完了
                int bytesSent = TcpClient.EndSend(ar);

                //LogProxy.Instance.Print($"client -> 「{bytesSent} byte」をServerへ送信");
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"Cliten: OnSpecificClientSendCallback Error—{ex.Message}");
            }
        }
    }

    public partial class ChatClient
    {
        public async Task SendMessageToClient(string targetName, string message)
        {
            if (client_to_server is not null)
            {
                var senderName = base.CharacterName;
                var jsonString = JsonSerialize(new ClientMessage(senderName, targetName, message));
                byte[] byteData = Encoding.Unicode.GetBytes($"{jsonString}" + "<EOF>");
                client_to_server.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(OnSpecificClientSendCallback), client_to_server);

                await Task.Yield();
            }
        }
    }
}
