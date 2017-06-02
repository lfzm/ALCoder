
using Newtonsoft.Json;

namespace Zzll.Net.Framework
{
    /// <summary>
    /// 请求对象
    /// </summary>
    public class Request
    {
        /// <summary>
        /// 操作编号
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 操作代码
        /// </summary>
        public string code { get; set; } 
        /// <summary>
        /// 功能报文
        /// </summary>
        public string para { get; set; }
        /// <summary>
        /// 请求渠道
        /// </summary>
        [JsonIgnore]
        public RequestChannel channel { get; set; }

    }

    /// <summary>
    /// 消息渠道
    /// </summary>
    public enum RequestChannel
    {
        /// <summary>
        /// Tcp服务监听
        /// </summary>
        TcpService,
        /// <summary>
        /// Tcp客户端订阅
        /// </summary>
        TcpClient,
        /// <summary>
        ///RedisMQ 消息队列
        /// </summary>
        RedisMQ,
        /// <summary>
        /// 定时处理监听
        /// </summary>
        Timing
    }
}
