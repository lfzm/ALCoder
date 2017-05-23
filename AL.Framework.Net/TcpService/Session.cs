using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zzll.Net.Framework.TcpService
{
    /// <summary>
    /// 回话上下文
    /// </summary>
    public class Session
    {
        #region -----构造方法-----
        /// <summary>
        /// 实例化客户对象
        /// </summary>
        /// <param name="client">套接字接口</param>
        /// <param name="ReceiveBufferSize">接收数据缓冲区大小</param>
        public Session(System.Net.Sockets.Socket client,int ReceiveBufferSize)
        {
            this.client = client;
            this.ReceiveDataBuffer =new  byte[ReceiveBufferSize];
            Id = (int)this.client.Handle;
            connIp = ((System.Net.IPEndPoint)client.RemoteEndPoint).Address.ToString();
            port = ((System.Net.IPEndPoint)client.RemoteEndPoint).Port;
            _SendDataCount = 0;
            _ReceiveDataCount = 0;
            LastTime = DateTime.Now;
        }

        /// <summary>
        /// 释放内存
        /// </summary>
        public void Dispose()
        {
            this.Dispose();
        }
        #endregion

        #region -------属性-------

        private int id;
        /// <summary>
        /// 会话ID
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private System.Net.Sockets.Socket client;
        /// <summary>
        /// 套接字接口
        /// </summary>
        public System.Net.Sockets.Socket Client
        {
            get { return client; }
        }

        private string connIp;
        /// <summary>
        /// 连接IP
        /// </summary>
        public string ConnIp
        {
            get { return connIp; }
        }

        private int port;
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        private int _SendDataCount;
        /// <summary>
        /// 发送字节
        /// </summary>
        public int SendDataCount
        {
            get { return _SendDataCount; }
            set { _SendDataCount = value; LastTime = DateTime.Now; }
        }

        private int _ReceiveDataCount;
        /// <summary>
        /// 接收字节
        /// </summary>
        public int ReceiveDataCount
        {
            get { return _ReceiveDataCount; }
            set { _ReceiveDataCount = value;LastTime = DateTime.Now; }
        }
        /// <summary>
        /// 接收报文片段存储
        /// </summary>
        public StringBuilder Datagram = new StringBuilder();
        /// <summary>
        /// 接收数据缓冲区
        /// </summary>
        public byte[] ReceiveDataBuffer { get; set; }
        /// <summary>
        /// 最后操作时间
        /// </summary>
        public DateTime LastTime { get; set; }
        #endregion
    }
}
