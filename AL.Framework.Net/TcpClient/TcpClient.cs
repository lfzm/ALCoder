
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Zzll.Net.Framework.Helper;

namespace Zzll.Net.Framework.TcpClient
{
    /// <summary>
    /// TCP客户端  工具类
    /// </summary>
    public class TcpClient : DatagramResolver
    {
        #region 构造方法------------
        /// <summary>
        /// TCP通讯对象
        /// </summary>
        public TcpClient() { Initialize("", 0, EncodingMothord.Default); }

        /// <summary>
        /// TCP通讯对象 
        /// </summary>
        /// <param name="Ip">服务器地址</param>
        /// <param name="Port">服务器端口</param>
        public TcpClient(string Ip, ushort Port)
        {
            Initialize(Ip, Port, EncodingMothord.Default);
        }
        /// <summary>
        /// TCP通讯对象 
        /// </summary>
        /// <param name="Ip">服务器地址</param>
        /// <param name="Port">服务器端口</param>
        /// <param name="encodmode">编码对象(枚举)</param>
        public TcpClient(string Ip, ushort Port, EncodingMothord encodmode)
        {
            Initialize(Ip, Port, encodmode);
        }
        /// <summary>
        /// 初始化TCP通讯对象
        /// </summary>
        public void Initialize(string Ip, ushort Port, EncodingMothord encodmode)
        {
            this.Ip = Ip;
            this.Port = Port;
            encod = new EncodHelper(encodmode);
        }
        #endregion

        #region 属性----------------
        /// <summary>
        /// Socket通讯对象
        /// </summary>
        private System.Net.Sockets.Socket sock = null;
        /// <summary>
        /// 服务器地址
        /// </summary>
        private string Ip { get; set; }
        /// <summary>
        /// 服务器端口
        /// </summary>
        private ushort Port { get; set; }

        private bool isConnection;
        /// <summary>
        /// 是否连接正常
        /// </summary>
        public bool IsConnection
        {
            get { return isConnection; }
            set { isConnection = value; }
        }

        private int _LoadProt;
        /// <summary>
        /// 获取本机端口
        /// </summary>
        public int LoadProt
        {
            get { return _LoadProt; }
        }


        private int _SendDataCount;
        /// <summary>
        /// 发送字节
        /// </summary>
        public int SendDataCount
        {
            get { return _SendDataCount; }
            set { _SendDataCount = value; }
        }

        private int _ReceiveDataCount;
        /// <summary>
        /// 接收字节
        /// </summary>
        public int ReceiveDataCount
        {
            get { return _ReceiveDataCount; }
            set { _ReceiveDataCount = value; }
        }

        ///////////////////////////////////////
        ///报文相关属性
        ///////////////////////////////////////
        /// <summary>
        /// 接收数据缓冲区大小64K
        /// </summary>
        private const int ReceiveBufferSize = 64 * 1024;
        /// <summary>
        /// 接收数据缓冲区
        /// </summary>
        private byte[] ReceiveDataBuffer = new byte[ReceiveBufferSize];
        /// <summary>
        /// 通讯格式编码解码器
        /// </summary>
        private EncodHelper encod;


        ///////////////////////////////////////
        ///重连器相关属性
        ///////////////////////////////////////
        private bool _IsReconnect = false;
        /// <summary>
        /// 是否需要重连(只支持本客户端匹配服务)
        /// </summary>
        public bool IsReconnect
        {
            get { return _IsReconnect; }
            set { _IsReconnect = value; }
        }

        private int reconnectTime = 5000;
        /// <summary>
        /// 重连时间间隔(单位:毫秒,默认为5秒)
        /// </summary>
        public int ReconnectTime
        {
            get { return reconnectTime; }
            set { reconnectTime = value; }
        }
        /// <summary>
        /// 重连次数
        /// </summary>
        public int ReconnectCount = 0;

        ///////////////////////////////////////
        ///心跳器相关属性
        ///////////////////////////////////////
        /// <summary>
        /// 上次心跳时间
        /// </summary>
        private DateTime HeartbeatTime;

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
        /// 心跳线程
        /// </summary>
        System.Threading.Thread Heartbeat_Thread = null;
        #endregion

        #region 事件----------------
        public delegate void NetEvent(string msg,System.Net.Sockets. Socket tcp);
        public delegate void Dlgestr(string msg);
        /// <summary>
        /// 连接断开事件
        /// </summary>
        public event Dlgestr ClientConnCut;
        /// <summary>
        /// 连接成功事件
        /// </summary>
        public event Dlgestr ClientConn;
        /// <summary>
        /// 接收数据事件
        /// </summary>
        public event NetEvent ClientRecvData;
        #endregion

        #region 公共方法区----------

        #region 连接器
        /// <summary>
        /// 异步连接服务器
        /// </summary>
        /// <param name="ip">服务器IP地址</param>
        /// <param name="port">服务器端口</param>
        public virtual void Connect(string ip, int port)
        {
            this.Ip = Ip;
            this.Port = Port;
            Connect();
        }
        /// <summary>
        /// 异步连接服务器
        /// </summary>
        public virtual bool Connect()
        {
            if (IsConnection)
                return true;
            //判断是否设置服务器IP地址,和端口
            if (string.IsNullOrEmpty(this.Ip))
            {
                CutConnection("服务器IP地址为空");
                return false;
            }
            if (this.Port == 0)
            {
                CutConnection("服务器IP服务器端口为空");
                return false;
            }
            try
            {
                //实例化Socket通讯对象
                sock = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //生成一个网络端口
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(this.Ip), this.Port);
                //开始异步请求连接
                sock.BeginConnect(iep, new AsyncCallback(Connected), sock);
            }
            catch (SocketException ex)
            {
                //服务器断开
                CutConnection(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                //服务器断开
                CutConnection(ex.Message);
            }
            catch (Exception ex)
            {
                //服务器断开
                CutConnection(ex.Message);

            }
            return false;
        }
        /// <summary>
        /// 同步连接服务器
        /// </summary>
        /// <returns></returns>
        public virtual bool ConnectSyn()
        {
            if (this.isConnection)
                return true;

            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(this.Ip), this.Port);
            System.Net.Sockets.Socket tempSocket = new System.Net.Sockets.Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            tempSocket.ReceiveTimeout = 30000;
            try
            {
                tempSocket.Connect(ipe);
            }
            catch (Exception)
            {
                return false;
            }

            if (tempSocket.Connected)
            {
                sock = tempSocket;
                this.isConnection = true;
            }

            return isConnection;
        }
        #endregion

        #region 发送器

        /// <summary>
        /// 异步发送器
        /// </summary>
        /// <param name="datagram">报文</param>
        /// <returns>返回是否成功</returns>
        public virtual bool Send(string datagram)
        {
            //如果没有连接无法发送报文
            if (!IsConnection)
            {
                CutConnection("服务器未连接");
                return false;
            }

            try
            {
                //加上后缀
                byte[] data = encod.ToString(datagram + this.Suffix);
                IAsyncResult iar = sock.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendDataEnd), sock);
                return true;
            }
            catch (SocketException ex)
            {
                //服务器断开
                CutConnection(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                //服务器断开
                CutConnection(ex.Message);
            }
            catch (Exception ex)
            {
                //服务器断开
                CutConnection(ex.Message);

            }
            return false;
        }

        /// <summary>
        /// 发送数据并接收服务器响应(短连接)
        /// </summary>
        /// <param name="datagram">报文</param>
        /// <returns>返回接收报文和错误消息</returns>
        public virtual string SendSyn(string datagram)
        {
            string response = "";
            if (!this.isConnection)
                return "Fail-EX:服务器连接失败";
            try
            {
                //发送的字节数组
                Byte[] bytesSent = encod.ToString(datagram + this.Suffix);
                // Send data to server.
                sock.Send(bytesSent, bytesSent.Length, 0);
            }
            catch (Exception)
            {
                //关闭连接
                this.Close();
                return "Fail-EX:数据发送失败";

            }
            int bytes = 0;
            //接收服务端的响应消息
            try
            {
                bytes = sock.Receive(ReceiveDataBuffer, ReceiveDataBuffer.Length, 0);
            }
            catch (SocketException se)
            {
                response = "Fail-EX:" + se.Message;
            }
            catch (Exception ex)
            {
                response = "Fail-EX:" + ex.Message;
            }
            if (bytes > 0)
            {
                response = encod.ToString(ReceiveDataBuffer, bytes);
                //粘报处理
                string[] packets = Resolve(response);
                foreach (var pack in packets)
                {
                    if (string.IsNullOrEmpty(pack)) continue;
                    response = pack;
                }
            }
            //数据接收完成后直接关闭连接,节约资源
            try { Close(); }
            catch (Exception) { }

            //只要发送成功了就返回成功，不管响应是否成功
            return response;
        }
        #endregion

        /// <summary>
        /// 关闭连接
        /// </summary>
        public virtual void Close()
        {
            if (!isConnection)
            {
                ClientConnCut("已经成功断开连接");
                return;
            }
            //设置连接状态
            isConnection = false;
            //关闭连接并且释放Socket通讯对象资源
            if (sock != null)
            {
                //关闭数据的接受和发送
                sock.Shutdown(SocketShutdown.Both);
                //停止心跳
                this.IsHeartbeat = false;
                //停止重连
                this.IsReconnect = false;
                sock.Close();
                sock = null;
            }
            ClientConnCut("已经成功断开连接");
        }

        #endregion

        #region 异步处理方法区------
        /// <summary>
        /// 建立Tcp连接后处理函数
        /// </summary>
        /// <param name="iar">异步Socket</param>
        protected void Connected(IAsyncResult iar)
        {
            System.Net.Sockets.Socket socket = null;
            try
            {
                socket = (System.Net.Sockets.Socket)iar.AsyncState;
                socket.EndConnect(iar);

                //设置连接状态
                isConnection = true;
                //获取本机端口
                this._LoadProt = ((IPEndPoint)sock.LocalEndPoint).Port;
                //触发连接成功事件
                if (ClientConn != null) ClientConn("Success");

                //判断是否开启心跳机制
                if (this.IsHeartbeat)
                {
                    if (Heartbeat_Thread == null)
                    {
                        HeartbeatTime = DateTime.Now;
                        Heartbeat_Thread = new System.Threading.Thread(Heartbeat);
                        Heartbeat_Thread.IsBackground = true;
                        Heartbeat_Thread.Start();
                    }

                }

            }
            catch (SocketException e)
            {
                isConnection = false;
                //触发连接失败事件
                if (ClientConnCut != null)
                    ClientConnCut("Fail-EX:" + e.Message);
                return;
            }

            ///////////////////////////////////////////
            ///开启接收数据监控
            ///////////////////////////////////////////
            try
            {
                socket.BeginReceive(ReceiveDataBuffer, 0, ReceiveBufferSize, SocketFlags.None, new AsyncCallback(RecvData), socket);
            }
            catch (Exception e)
            {
                isConnection = false;
                //触发连接失败事件
                if (ClientConnCut != null)
                    ClientConnCut("Fail-EX:" + e.Message);
            }
        }

        /// <summary>
        /// 数据发送完成处理函数
        /// </summary>
        /// <param name="iar"></param>
        protected void SendDataEnd(IAsyncResult iar)
        {
            try
            {
                if (!isConnection)
                    return;
                System.Net.Sockets.Socket remote = (System.Net.Sockets.Socket)iar.AsyncState;
                int sent = remote.EndSend(iar);
                //累计发送报文字节
                _SendDataCount += sent;
                if (sent == 0)
                {
                    //发送失败,断开连接
                    CutConnection("报文发送失败");
                    return;
                }

            }
            catch (SocketException ex)
            {
                CutConnection(ex.Message);
                return;
            }
            catch (ObjectDisposedException ex)
            {
                CutConnection(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                CutConnection(ex.Message);
                return;
            }
        }

        /// <summary>
        /// 数据接收处理函数
        /// </summary>
        /// <param name="iar">异步Socket</param>
        protected void RecvData(IAsyncResult iar)
        {
            System.Net.Sockets.Socket client = null;
            try
            {
                if (!isConnection)
                    return;
                client = (System.Net.Sockets.Socket)iar.AsyncState;
                SocketError sockerr;
                int recv = client.EndReceive(iar, out sockerr);
                //累计接收报文字节
                _ReceiveDataCount += recv;
                if (recv == 0 || sockerr == SocketError.ConnectionReset)
                {
                    //客户端断开了连接
                    CutConnection("主机断开连接");
                    return;
                }
                //获取报文
                string Packet = encod.ToString(ReceiveDataBuffer, recv);

                //粘报处理
                string[] packets = Resolve(Packet);
                foreach (var pack in packets)
                {
                    if (string.IsNullOrEmpty(pack)) continue;

                    //判断是否为心跳报文
                    if (pack.Equals("0"))
                    {
                        //心跳
                        HeartbeatTime = DateTime.Now;
                        continue;
                    }
                    //通知接收数据事件
                    if (ClientRecvData != null) ClientRecvData(pack, sock);
                }
            }
            catch (SocketException ex)
            {
                CutConnection(ex.Message);
                return;
            }
            catch (ObjectDisposedException ex)
            {
                //服务器断开
                CutConnection(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                //服务器断开
                CutConnection(ex.Message);
                return;
            }
            try
            {
                //开启数据接收监听
                sock.BeginReceive(ReceiveDataBuffer, 0, ReceiveBufferSize, SocketFlags.None, new AsyncCallback(RecvData), sock);
            }
            catch (Exception ex)
            {
                //服务器断开
                CutConnection(ex.Message);
                return;
            }
        }

        /// <summary>
        /// 断开连接函数
        /// </summary>
        protected void CutConnection(string Messages)
        {
            //服务器断开
            this.isConnection = false;
            //关闭数据的接受和发送
            //关闭连接并且释放Socket通讯对象资源
            if (sock != null && sock.Connected)
            {
                //关闭数据的接受和发送
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
            }
            if (ClientConnCut != null)
                ClientConnCut("Fail-EX:" + Messages);
        }

        /// <summary>
        /// 心跳器函数
        /// </summary>
        protected void Heartbeat()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                //判断是否断开连接
                if (!isConnection)
                {
                    Reconnect();
                    continue;
                }
                //判断上次心跳时间是否大于现在30秒
                if ((DateTime.Now - HeartbeatTime).TotalMilliseconds > 30000)
                    CutConnection("网络异常,主机连接断开");
            }
        }

        /// <summary>
        /// 重新连接函数
        /// </summary>
        private void Reconnect()
        {
            if (!this.IsReconnect)
                return;
            IsReconnect = false;
            while (true)
            {
                System.Threading.Thread.Sleep(ReconnectTime);
                ClientConnCut("重新连接");
                //判断是否断开连接
                if (isConnection)
                {
                    IsReconnect = true;
                    ReconnectCount = 0;
                    return;
                }
                //已经断开连接,启动重连
                ReconnectCount++;
                this.Connect();
            }
        }
        #endregion
    }
}
