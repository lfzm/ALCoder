using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Zzll.Net.Framework.Helper;

namespace Zzll.Net.Framework.TcpService
{
    /// <summary>
    /// TCP服务端 工具类
    /// </summary>
    public class TcpService : DatagramResolver
    {
        #region 属性----------------
        /// <summary>
        /// 保存所有客户端会话的哈希表
        /// </summary>
        private Hashtable _SessionTable;
        /// <summary>
        /// 所有客户端会话集合
        /// </summary>
        public List<Session> SessionTable
        {
            get
            {
                List<Session> list = new List<Session>();
                try
                {

                    if (_SessionTable == null || _SessionTable.Count == 0)
                        return list;
                    //便利需要连接客户端
                    foreach (object key in new ArrayList(_SessionTable.Keys))
                    {
                        //获取需要发送报文的客户端
                        Session client = _SessionTable[key] as Session;
                        if (client == null)
                            continue;
                        list.Add(client);
                    }
                    return list;
                }
                catch (Exception)
                {
                    return list;
                }
            }
        }
        /// <summary>
        /// 默认的服务器最大连接客户端端数据
        /// </summary>
        public int DefaultMaxClient = 300;
        /// <summary>
        /// 服务器使用的异步Socket类,
        /// </summary>
        private System.Net.Sockets.Socket sock;
        /// <summary>
        /// 服务器程序使用的端口
        /// </summary>
        private ushort Port;
        /// <summary>
        /// 当前的连接的客户端数
        /// </summary>
        public int ClientCount
        {
            get
            {
                return _SessionTable.Count;
            }
        }
        private bool serverState;
        /// <summary>
        /// 服务状态
        /// </summary>
        public bool ServerState
        {
            get { return serverState; }
            set { serverState = value; }
        }

        ///////////////////////////////////////
        ///报文相关属性
        ///////////////////////////////////////

        /// <summary>
        /// 发送数据缓冲区大小8K
        /// </summary>
        private const int SendBufferSize = 8 * 1024;
        /// <summary>
        /// 接收数据缓冲区大小64K
        /// </summary>
        private const int ReceiveBufferSize = 64 * 1024;
        /// <summary>
        /// 通讯格式编码解码器
        /// </summary>
        private EncodHelper encod;

        ///////////////////////////////////////
        //心跳相关属性
        ///////////////////////////////////////
        private bool _IsHeartbeat = false;
        /// <summary>
        /// 是否开启心跳(只支持本客户端匹配服务)
        /// </summary>
        public bool IsHeartbeat
        {
            get { return _IsHeartbeat; }
            set { _IsHeartbeat = value; }
        }
        /// <summary>
        /// 每2.5秒心跳一次
        /// </summary>
        public int HeartbeatTime = 2500;
        /// <summary>
        /// 心跳线程
        /// </summary>
        System.Threading.Thread Heartbeat_Thread;
        #endregion

        #region 构造方法------------
        /// <summary>
        /// 构造方法
        /// </summary>
        public TcpService()
        {
            _SessionTable = new Hashtable();
            ServerState = false;
            encod = new EncodHelper(EncodingMothord.Default);
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_Port">端口号</param>
        public TcpService(ushort _Port)
        {
            Port = _Port;
            _SessionTable = new Hashtable();
            ServerState = false;
            encod = new EncodHelper(EncodingMothord.Default);
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_Port">端口号</param>
        /// <param name="encodmode">报文编码</param>
        public TcpService(ushort _Port, EncodingMothord encodmode)
        {
            Port = _Port;
            _SessionTable = new Hashtable();
            ServerState = false;
            encod = new EncodHelper(encodmode);
        }
        #endregion

        #region 事件----------------
        public delegate void NetEvent(string msg, Session e);
        public delegate void NetClientEvent(Session e);
        public delegate void Dlgestr(string msg);
        /// <summary>
        /// 客户端关闭事件
        /// </summary>
        public event NetClientEvent ClientClose;
        /// <summary>
        /// 客户端建立连接事件
        /// </summary>
        public event NetClientEvent ClientConn;
        /// <summary>
        /// 服务器接收到数据事件
        /// </summary>
        public event NetEvent ClientRecvData;
        /// <summary>
        /// 发送客户端事件
        /// </summary>
        public event NetEvent ClientSandData;
        /// <summary>
        /// 服务器已经满事件
        /// </summary>
        public event Dlgestr ServerFull;
        /// <summary>
        /// 服务器处理错误
        /// </summary>
        public event Dlgestr ServerError;
        /// <summary>
        /// 服务启动事件
        /// </summary>
        public event Dlgestr ServerStart;
        /// <summary>
        /// 服务停止事件
        /// </summary>
        public event Dlgestr ServerStop;
        #endregion

        #region 公共方法------------

        /// <summary>
        /// 启动服务器程序,开始监听客户端请求
        /// </summary>
        /// <param name="_Port">端口</param>
        public virtual void Start(ushort _Port)
        {
            this.Port = _Port;
            this.Start();
        }
        /// <summary>
        /// 启动服务器程序,开始监听客户端请求
        /// </summary>
        public virtual void Start()
        {
            //判断服务状态
            if (ServerState)
                return;
            if (Port == 0)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:开启服务;请先指定服务端口");
            }
            //生成Socket对象
            sock = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定端口
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, this.Port);
            sock.Bind(iep);
            //开始监听
            sock.Listen(5);
            //设置异步方法接受客户端连接
            sock.BeginAccept(new AsyncCallback(AcceptConn), sock);
            ServerState = true;
            //发送连接成功事件
            if (ClientConn != null)
                ServerStart("Success");

            //判断是否开启心跳机制
            if (this.IsHeartbeat)
            {
                Heartbeat_Thread = new System.Threading.Thread(Heartbeat);
                Heartbeat_Thread.IsBackground = true;
                Heartbeat_Thread.Start();
            }

        }

        /// <summary>
        /// 停止监听服务
        /// </summary>
        public virtual void Stop()
        {
            //停止接收数据和发送数据
            if (!this.ServerState)
                return;
            //关闭数据连接,负责客户端会认为是强制关闭连接
            if (sock.Connected)
                sock.Shutdown(SocketShutdown.Both);

            //关闭服务
            this.ServerState = false;
            sock.Close();
            sock = null;

            //端口所有客户端连接
            foreach (object key in new ArrayList(_SessionTable.Keys))
            {
                //获取需要发送报文的客户端
                Session client = _SessionTable[key] as Session;
                //关闭数据的接受和发送
                client.Client.Shutdown(SocketShutdown.Both);
                client.Client.Close();
            }
            //清空客户端连接
            _SessionTable.Clear();

            //发送断开连接成功事件
            if (ServerStop != null)
                ServerStop("服务已停止");
        }

        /// <summary>
        /// 断开客户端连接
        /// </summary>
        /// <param name="client">Socket对象</param>
        public virtual void Close(System.Net.Sockets.Socket client)
        {
            Session s = FindSession(client);
            if (s == null)
                client.Close();

            else
            {
                //关闭数据的接受和发送
                try
                {
                    s.Client.Shutdown(SocketShutdown.Both);
                    s.Client.Close();
                }
                catch (Exception)
                {
                }

                //移除客户端连接
                lock (_SessionTable)
                {
                    _SessionTable.Remove(s.Id);
                }
                //通知客户端连接事件 
                if (ClientClose != null)
                    ClientClose(s);
            }
        }

        /// <summary>
        /// 通过Socket对象查找Session对象
        /// </summary>
        /// <param name="client">Socket对象</param>
        /// <returns>找到的Session对象,如果为null,说明并不存在该回话</returns>
        public Session FindSession(System.Net.Sockets.Socket client)
        {
            return (Session)_SessionTable[(int)client.Handle];
        }
        /// <summary>
        /// 通过话柄编号查找Session对象
        /// </summary>
        /// <param name="clientId">话柄编号</param>
        /// <returns>找到的Session对象,如果为null,说明并不存在该回话</returns>
        public Session FindSession(int clientId)
        {
            return (Session)_SessionTable[clientId];
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="s">发送对象</param>
        /// <param name="datagram">报文</param>
        public virtual void Send(Session s, string datagram)
        {
            //如果没有连接无法发送报文
            if (!ServerState)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:发送数据;服务器未连接");
                return;
            }
            try
            {
                //加上后缀
                byte[] data = encod.ToString(datagram + this.Suffix);
                IAsyncResult iar = s.Client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendDataEnd), s);
                return;
            }
            catch (SocketException ex)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:发送数据;" + ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:发送数据;" + ex.Message);
            }
            catch (Exception ex)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:发送数据;" + ex.Message);
            }
            return;
        }

        /// <summary>
        /// 广播报文
        /// </summary>
        /// <param name="datagram">报文</param>
        public virtual void Televise(string datagram)
        {
            //遍历需要连接客户端
            foreach (object key in new ArrayList(_SessionTable.Keys))
            {
                //获取需要发送报文的客户端
                Session client = _SessionTable[key] as Session;
                if (client == null)
                    continue;
                //发送报文
                Send(client, datagram);
            }
        }

        #endregion

        #region 异步处理方法区------
        /// <summary>
        /// 客户端连接处理函数
        /// </summary>
        /// <param name="iar">欲建立服务器连接的Socket对象</param>
        protected virtual void AcceptConn(IAsyncResult iar)
        {
            try
            {
                //如果服务器停止了服务,就不能再接收新的客户端
                if (!this.ServerState)
                    return;

                //接受一个客户端的连接请求 
                System.Net.Sockets.Socket oldserver = (System.Net.Sockets.Socket)iar.AsyncState;
                System.Net.Sockets.Socket client = oldserver.EndAccept(iar);

                //检查是否达到最大的允许的客户端数目
                if (this.ClientCount >= this.DefaultMaxClient)
                {
                    //服务器已满,发出通知
                    if (ServerFull != null)
                    {
                        ServerFull("服务连接数已满");
                        //继续接收来自来客户端的连接
                        sock.BeginAccept(new AsyncCallback(AcceptConn), sock);
                        client.Close();
                        return;
                    }
                }

                //添加到连接存储器中
                Session s = new Session(client, ReceiveBufferSize);
                _SessionTable.Add(s.Id, s);

                if (ClientConn != null)
                    ClientConn(s);
                try
                {
                    //接着开始异步连接客户端
                    client.BeginReceive(s.ReceiveDataBuffer, 0, s.ReceiveDataBuffer.Length, SocketFlags.None,
                     new AsyncCallback(ReceiveData), s);
                }
                catch (SocketException ex)
                {
                    if (ServerError != null)
                        ServerError("Fail-EX:客户端连接;ErrorCode:" + ex.ErrorCode + ex.Message);
                }
                catch (Exception ex)
                {
                    if (ServerError != null)
                        ServerError("Fail-EX:客户端连接;" + ex.Message);
                }

                //继续接收来自来客户端的连接
                sock.BeginAccept(new AsyncCallback(AcceptConn), sock);
            }
            catch (Exception ex)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:客户端连接;" + ex.Message);
            }
        }

        /// <summary>
        /// 数据接收处理函数
        /// </summary>
        /// <param name="iar">目标客户端Socket</param>
        protected virtual void ReceiveData(IAsyncResult iar)
        {
            Session s = (Session)iar.AsyncState;
            System.Net.Sockets.Socket client = s.Client;
            try
            {
                //如果两次开始了异步的接收,所以当客户端退出的时候,会两次执行EndReceive 
                SocketError sockerr;
                int recv = client.EndReceive(iar, out sockerr);
                if (recv == 0 || sockerr == SocketError.ConnectionReset)
                {
                    //客户端断开了连接
                    this.Close(client);
                    return;
                }
                //获取报文
                s.Datagram.Append(encod.ToString(s.ReceiveDataBuffer, recv));

                //累计接收报文字节
                s.ReceiveDataCount += recv;

                //粘报处理
                string[] packets = Resolve(s);
                foreach (var pack in packets)
                {
                    if (string.IsNullOrEmpty(pack)) continue;
                    //通知接收数据事件
                    if (ClientRecvData != null) ClientRecvData(pack, s);
                }

            }
            catch (SocketException ex)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:接收数据;ErrorCode:" + ex.ErrorCode + ex.Message);

            }
            catch (ObjectDisposedException ex)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:接收数据;" + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:接收数据;" + ex.Message);
                return;
            }
            try
            {
                //开启数据接收监听
                client.BeginReceive(s.ReceiveDataBuffer, 0, s.ReceiveDataBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveData), s);
            }
            catch (Exception ex)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:开启数据接收;" + ex.Message);
                return;
            }
        }

        /// <summary>
        /// 发送数据完成处理函数
        /// </summary>
        /// <param name="iar">目标客户端Socket</param>
        protected virtual void SendDataEnd(IAsyncResult iar)
        {
            try
            {
                if (!this.ServerState)
                    return;
                Session s = (Session)iar.AsyncState;
                int sent = s.Client.EndSend(iar);
                if (sent == 0)
                {
                    if (ServerError != null)
                        ServerError("Fail-EX:发送数据;报文发送失败");
                    return;
                }
                s.LastTime = DateTime.Now;
                //获取连接的客户端
                if (this.ClientSandData != null)
                {
                    //累计发送报文字节
                    s.SendDataCount += sent;

                    ClientSandData("发送成功", s);
                }
            }
            catch (SocketException ex)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:发送数据;ErrorCode:" + ex.ErrorCode + ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:发送数据;" + ex.Message);
            }
            catch (Exception ex)
            {
                if (ServerError != null)
                    ServerError("Fail-EX:发送数据;" + ex.Message);
            }
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        private void Heartbeat()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(HeartbeatTime);
                //判断是否断开连接
                if (!ServerState)
                    continue;
                //发送心跳包
                //遍历需要连接客户端
                foreach (object key in new ArrayList(_SessionTable.Keys))
                {
                    //获取需要发送报文的客户端
                    Session client = _SessionTable[key] as Session;
                    if (client == null)
                        continue;
                    //判断30秒没有操作就发送心跳
                    if (client.LastTime.AddSeconds(30) > DateTime.Now)
                        continue;
                    //发送报文
                    Send(client, "0");
                }
            }
        }
        #endregion

    }
}
