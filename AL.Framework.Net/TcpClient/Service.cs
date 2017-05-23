using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zzll.Net.Framework.TcpClient
{
    /// <summary>
    /// Tcp客户端处理器
    /// </summary>
    public class Service : BaseHandle<Config>
    {
        /// <summary>
        /// TCP客户端
        /// </summary>
        public TcpClient cli { get; set; }
        /// <summary>
        /// 启动服务
        /// </summary>
        public override void Start(Func<Request, Response> func)
        {
            base.Handle_Event = func;

            cli = new TcpClient(base.config.Ip, base.config.Port, config.Encoding);
            cli.ClientConn += Cli_ClientConn;
            cli.ClientConnCut += Cli_ClientConnCut;
            cli.ClientRecvData += Cli_ClientRecvData;
            cli.Connect();
        }

        /// <summary>
        /// 接受数据事件
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="tcp">会话上下文</param>
        private void Cli_ClientRecvData(string msg, System.Net.Sockets.Socket tcp)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 断开连接事件
        /// </summary>
        /// <param name="msg"></param>
        private void Cli_ClientConnCut(string msg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 连接成功事件
        /// </summary>
        /// <param name="msg"></param>
        private void Cli_ClientConn(string msg)
        {
            throw new NotImplementedException();
        }

       
    }
}
