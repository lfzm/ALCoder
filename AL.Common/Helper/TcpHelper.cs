#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：Tcp请求帮助类
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Web;

namespace AL.Common.Helper
{
    public class TcpHelper : IDisposable
    {
        /// <summary>
        /// 接收数据的字节数组长度
        /// </summary>
        private int recbyteCount = 10000;

        /// <summary>
        /// Session内部用的Socket对象
        /// </summary>
        private Socket socket;

        private bool isConnected;

        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int SPort { get; set; }

        #region 构造器
        public TcpHelper() { }
        public TcpHelper(string sip, int sport)
        {
            ServerIP = sip;
            SPort = sport;
        }
        #endregion

        #region 连接服务器
        /// <summary>
        /// 使用带参构造方法后可以直接使用的连接方法
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            return ConnectSocket(ServerIP, SPort);
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool ConnectSocket(string server, int port)
        {
            if (isConnected)
                return true;

            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(server), port);
            Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            tempSocket.ReceiveTimeout = 60000;
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
                socket = tempSocket;
                isConnected = true;
            }

            return isConnected;
        }
        #endregion

        #region 关闭连接
        public void Close()
        {
            socket.Close();
            isConnected = false;
        }
        #endregion

        /// <summary>
        /// 获取Socket连接的本地端口
        /// </summary>
        /// <returns></returns>
        private string GetLocalEndPoint()
        {
            return socket.LocalEndPoint.ToString();
        }

        public void SendDataOnly(string sdata)
        {
            if (!isConnected)
                return;

            Byte[] bytesSent = Encoding.GetEncoding("gb2312").GetBytes(sdata);
            Byte[] bytesReceived = new Byte[recbyteCount];

            // Send request to the server.
            socket.Send(bytesSent, bytesSent.Length, 0);
        }

        #region 发送数据并接收服务器响应
        /// <summary>
        /// 发送消息并获取服务端响应
        /// </summary>
        /// <returns>是否发送成功</returns>
        public string SendData(string sdata, Encoding Encod)
        {
            string response = "";
            if (!isConnected)
                return "Fail-EX:服务器连接失败";
            try
            {
                //发送的字节数组
                Byte[] bytesSent = Encod.GetBytes(sdata);
                // Send data to server.
                socket.Send(bytesSent, bytesSent.Length, 0);
            }
            catch (Exception)
            {
                return "Fail-EX:数据发送失败";
            }

            //接收数据的缓冲区
            Byte[] bytesReceived = new Byte[recbyteCount];

            int bytes = 0;
            //接收服务端的响应消息
            try
            {
                bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054)   //连接被重置
                    isConnected = false;
                response = "Fail-EX:" + se.Message;
            }
            catch (Exception ex)
            {
                isConnected = false;
                response = "Fail-EX:" + ex.Message;
            }

            if (bytes > 0)
                response = Encod.GetString(bytesReceived, 0, bytes);

            //数据接收完成后直接关闭连接,节约资源
            try { Close(); }
            catch (Exception) { }

            //只要发送成功了就返回成功，不管响应是否成功
            return response;
        }

        #endregion



        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
