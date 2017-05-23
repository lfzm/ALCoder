using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AL.Common.Helper;
using AL.Common.Models;
using AL.Common.Modules.LogModule;

namespace AL.DbContext.Tcp
{
    /// <summary>
    /// Tcp 客户端
    /// </summary>
    public class TcpContextBase
    {
        /// <summary>
        /// 日志记录提供者
        /// </summary>
        private Action<string, string, string> LoggerProvider { get; set; }

        TcpHelper tcp;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">服务IP</param>
        /// <param name="port">服务端口</param>
        public TcpContextBase(string ip, int port)
        {
            tcp = new TcpHelper(ip, port);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">服务IP</param>
        /// <param name="port">服务端口</param>
        /// <param name="logger">日志记录器</param>
        public TcpContextBase(string ip, int port, Action<string, string, string> logger) : this(ip, port)
        {
            LoggerProvider = logger;
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encod"></param>
        /// <returns></returns>
        public TResult Send<TResult>(string data, Encoding encod) where TResult : Result, new()
        {
            if(!tcp.Connect())
                return GetErrorResult<TResult>("请求处理失败");
            string Message = tcp.SendData(data, encod);
            if (LoggerProvider != null)
                LoggerProvider(string.Format("ip={0}&prot={1}", tcp.ServerIP, tcp.SPort), data, Message);
            if (Message.Contains("Fail-EX:"))
                return GetErrorResult<TResult>("请求处理失败");

            //解析成结果对象
            return Message.Replace(">","").ConvertToResult<TResult>();
        }


        /// <summary>
        /// 返回错误对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        private TResult GetErrorResult<TResult>(string message) where TResult : Result, new()
        {
            TResult res = new TResult();
            res.Message = message;
            res.Status = (int)ResultTypes.InnerError;
            res.Success = false;
            return res;
        }
    }
}
