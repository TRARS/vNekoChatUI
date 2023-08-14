using Common.WPF;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vNekoChatUI.Character.SocketUtils
{
    //服务端
    public partial class ChatServer : ChatBase
    {
        //SemaphoreSlimのインスタンスを生成
        private readonly SemaphoreSlim allDone = new SemaphoreSlim(0, 1);

        //字典，新客户端连接时，往 _socketMap 添加该客户端
        //                    ，往 resetEventMap 添加信号量
        //private Dictionary<string, ManualResetEvent> _resetEventMap = new();// address -> ManualResetEvent
        private Dictionary<string, Socket> _socketMap = new();     // address -> Socket
        private Dictionary<string, string> _userMap = new();       // name -> address
        private Dictionary<string, string> _userMapReverse = new();// addres -> sname 

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChatServer(string name, int age = 18)
        {
            base.CharacterName = $"{name}";
            base.CharacterAge = $"{age}";
        }

        //TCP/IPの接続開始処理
        public async Task<bool> StartListening(string address, int port, Action<int> port_act)
        {
            // IPアドレスとポート番号を指定して、ローカルエンドポイントを設定
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(address), port);

            // TCP/IPのソケットを作成
            Socket TcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //
            int retryCount = 0;

            while (true)
            {
                if (retryCount++ > 10)
                {
                    LogProxy.Instance.Print("★server -> サーバー初期化できませんでした");
                    break;
                }

                try
                {
                    TcpServer.Bind(localEndPoint);  // TCP/IPのソケットをローカルエンドポイントにバインド
                    TcpServer.Listen(32);           // 待ち受け開始

                    port_act.Invoke(((IPEndPoint)TcpServer!.LocalEndPoint!).Port);

                    await Task.Run(() =>
                    {
                        while (true)
                        {
                            // 非同期ソケットを開始して、接続をリッスンする
                            LogProxy.Instance.Print("★server -> 接続待機中...");
                            TcpServer.BeginAccept(new AsyncCallback(InitAcceptCallback), TcpServer);

                            // シグナル状態になるまで待機
                            allDone.Wait();
                        }
                    });
                }
                catch (Exception ex)
                {
                    LogProxy.Instance.Print($"StartListening Error—{ex.Message}");
                }
            }

            return false;
        }

        private void InitAcceptCallback(IAsyncResult ar)
        {
            // シグナル状態にし、メインスレッドの処理を続行する
            allDone.Release();

            // クライアント要求を処理するソケットを取得
            Socket TcpServer = (Socket)ar.AsyncState;
            Socket TcpClient = TcpServer.EndAccept(ar);

            LogProxy.Instance.Print($"★server -> Client({TcpClient.RemoteEndPoint})が接続した");

            // 端末からデータ受信を待ち受ける
            StateObject state = new StateObject();
            state.workSocket = TcpClient;
            TcpClient.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(InitReceiveCallback), state);
        }
        private void InitReceiveCallback(IAsyncResult ar)
        {
            var content = string.Empty;

            try
            {
                // 非同期オブジェクトからソケット情報を取得
                StateObject state = (StateObject)ar.AsyncState;
                Socket TcpClient = state.workSocket;

                // クライアントソケットからデータを読み取り
                int bytesRead = TcpClient.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // 受信したデータを蓄積
                    state.sb.Append(Encoding.Unicode.GetString(state.buffer, 0, bytesRead));

                    // 蓄積データの終端タグを確認
                    content = state.sb.ToString();

                    if (content.IndexOf("<EOF>") > -1)
                    {
                        // 終了タグ<EOF>があれば、読み取り完了
                        LogProxy.Instance.Print($"★server -> Client({TcpClient.RemoteEndPoint})曰く「{content}」");

                        // ログイン認証
                        var jsonObject = this.JsonDeserialize<ClientInitialAuthentication>(content.TrimEnd("<EOF>".ToCharArray()));

                        // ログインメッセージ
                        var loginName = $"{jsonObject.Name}";
                        var loginMessage = $"{jsonObject.Name}({TcpClient.RemoteEndPoint})がサーバーにログインした";
                        var loginReply = JsonSerialize(new ClientMessage(base.CharacterName, loginName, loginMessage));
                        // 
                        _userMap.TryAdd(jsonObject.Name, $"{TcpClient.RemoteEndPoint}");
                        _userMapReverse.TryAdd($"{TcpClient.RemoteEndPoint}", jsonObject.Name);

                        // ASCIIコードをバイトデータに変換
                        byte[] byteData = Encoding.Unicode.GetBytes($"{loginReply}" + "<EOF>");

                        // クライアントへデータの送信を開始
                        TcpClient.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(InitSendCallback), TcpClient);
                    }
                    else
                    {
                        // 取得していないデータがあるので、受信再開
                        TcpClient.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(InitReceiveCallback), state);
                    }
                }
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"InitReceiveCallback Error—{ex.Message}");
            }
        }
        private void InitSendCallback(IAsyncResult ar)
        {
            try
            {
                // 非同期オブジェクトからソケット情報を取得
                Socket TcpClient = (Socket)ar.AsyncState;

                // クライアントへデータ送信完了
                int bytesSent = TcpClient.EndSend(ar);
                //LogProxy.Instance.Print("server -> 「OK」をClientへ送信");

                //ソケット通信を終了
                //LogProxy.Instance.Print("接続終了");
                //TcpClient.Shutdown(SocketShutdown.Both);
                //TcpClient.Close();

                //将客户端信息写入两字典 TcpClient.RemoteEndPoint
                OnNewClientConnected(TcpClient);
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"InitSendCallback Error—{ex.Message}");
            }
        }
    }

    //服务端收到消息时的回调函数
    public partial class ChatServer
    {
        //新客户端加入，安排独立线程持续接收[该客户端]的消息 ← 安排个毛线独立线程
        private void OnNewClientConnected(Socket client)
        {
            string key = client.RemoteEndPoint!.ToString()!;
            //_resetEventMap.TryAdd(key, new ManualResetEvent(false));
            _socketMap.TryAdd(key, client);

            //每个客户端安排一个线程
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        _resetEventMap[key].Reset();
            //        {
            //            Socket TcpClient = _socketMap[key];

            //            // 端末からデータ受信を待ち受ける
            //            StateObject state = new StateObject();
            //            state.workSocket = TcpClient;
            //            state.key = key;
            //            TcpClient.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnNewClientReceiveCallback_Forwarded), state);
            //        }
            //        _resetEventMap[key].WaitOne();
            //    }
            //});


            // 端末からデータ受信を待ち受ける
            StateObject state = new StateObject();
            state.workSocket = client;
            //state.key = key;
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnNewClientReceiveCallback_Forwarded), state);

        }
        //收到[该客户端]消息后，执行该回调函数（具体行为：解析JSON中的目标客户端地址，将消息转发给指定客户端）
        private void OnNewClientReceiveCallback_Forwarded(IAsyncResult ar)
        {
            try
            {
                // 非同期オブジェクトからソケット情報を取得
                StateObject state = (StateObject)ar.AsyncState;
                Socket TcpClient = state.workSocket;
                //string key = state.key;

                // クライアントソケットからデータを読み取り
                int bytesRead = TcpClient.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // 受信したデータを蓄積
                    state.sb.Append(Encoding.Unicode.GetString(state.buffer, 0, bytesRead));

                    // 蓄積データの終端タグを確認
                    var content = state.sb.ToString();

                    // 終了タグ<EOF>があれば、読み取り完了
                    if (content.EndsWith("<EOF>"))
                    {
                        state.sb.Clear();//已拿到完整消息，缓存可以清空了

                        // 解析Json，获取 sender、receiver、message
                        var jsonObject = this.JsonDeserialize<ClientMessage>(content.TrimEnd("<EOF>".ToCharArray()));
                        if (jsonObject != null)
                        {
                            _userMap.TryGetValue(jsonObject.ReceiverName, out string? _receiverAddress);//获得收件人地址
                            _userMap.TryGetValue(jsonObject.SenderName, out string? _senderAddress);    //获得发件人地址
                            if (_receiverAddress is not null && _senderAddress is not null)
                            {
                                var senderName = jsonObject.SenderName;
                                var senderAddress = _senderAddress.ToString();
                                var senderMessage = jsonObject.Message;

                                var receiverName = jsonObject.ReceiverName;
                                var receiverAddress = _receiverAddress.ToString();
                                var receiverClient = _socketMap[_receiverAddress]; // 获得收件人socket

                                var jsonString = JsonSerialize(new ClientMessage(senderName, receiverName, senderMessage));
                                byte[] byteData = Encoding.Unicode.GetBytes($"{jsonString}" + "<EOF>");
                                receiverClient.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(OnSpecificClientSendCallback), receiverClient);
                            }
                            else
                            {
                                if (jsonObject.ReceiverName == this.CharacterName)
                                {
                                    System.Windows.MessageBox.Show($"「{this.CharacterName}」にメッセージを送っちゃダメだよ");
                                }
                                else
                                {
                                    System.Windows.MessageBox.Show($"送信先のユーザー「{jsonObject.ReceiverName}」がサーバーにログインしていない");
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Server: json解析失败");
                        }

                        //_resetEventMap[key].Set();
                    }
                    //else
                    {
                        // 取得していないデータがあるので、受信再開
                        TcpClient.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnNewClientReceiveCallback_Forwarded), state);
                    }
                }
                else
                {
                    // ここにまで辿り着けないよう祈る
                    throw new InvalidOperationException("Server: The code should not reach this point.");
                }
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"Server: OnNewClientReceiveCallback_Forwarded Error—{ex.Message}");
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

                //LogProxy.Instance.Print($"server -> 「{bytesSent} byte」をClientへ送信");
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"Server: OnSpecificClientSendCallback Error—{ex.Message}");
            }
        }
    }

    //服务端主动向客户端发消息
    public partial class ChatServer
    {
        public void BroadcastMessageToAllClients(string message)
        {
            foreach (var server_to_client in _socketMap.Values)
            {
                var jsonString = JsonSerialize(new ClientMessage(base.CharacterName, "親愛なるあなた", message));
                byte[] byteData = Encoding.Unicode.GetBytes($"{jsonString}" + "<EOF>");
                server_to_client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(OnSpecificClientSendCallback), server_to_client);
            }
        }

        public void SendMessageToClient(string targetName, string message)
        {
            _userMap.TryGetValue(targetName, out string? _receiverAddress);//获得收件人地址
            _socketMap.TryGetValue(_receiverAddress ?? "", out var server_to_client);
            if (_receiverAddress is not null && server_to_client is not null)
            {
                var jsonString = JsonSerialize(new ClientMessage(base.CharacterName, targetName, message));
                byte[] byteData = Encoding.Unicode.GetBytes($"{jsonString}" + "<EOF>");
                server_to_client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(OnSpecificClientSendCallback), server_to_client);
            }
        }
    }
}
