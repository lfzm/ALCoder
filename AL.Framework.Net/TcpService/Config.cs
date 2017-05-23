
using System.Collections.Generic;
using Zzll.Net.Framework.Helper;

namespace Zzll.Net.Framework.TcpService
{
    /// <summary>
    /// TcpService 配置
    /// </summary>
    public class Config
    {
        private string _suffix = ">";
        private EncodingMothord _encoding = EncodingMothord.UTF8;
        private int _defaultMaxClient = 500;
        private bool _isHeartbeat = false;
        public Config()
        {
            //初始化
            this.ClientWhiteList = new List<ClientWhite>();
        }

        /// <summary>
        /// 服务端口
        /// </summary>
        public ushort Port { get; set; }
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int DefaultMaxClient
        {
            get { return _defaultMaxClient; }
            set { _defaultMaxClient = value; }
        }
        /// <summary>
        /// 连接客户端白名单
        /// </summary>
        public List<ClientWhite> ClientWhiteList { get; set; }
        /// <summary>
        /// 报文后缀
        /// </summary>
        public string Suffix
        {
            get { return _suffix; }
            set { _suffix = value; }
        }
        /// <summary>
        /// 消息编码
        /// </summary>
        public EncodingMothord Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }
        /// <summary>
        /// 是否开启心跳
        /// </summary>
        public bool IsHeartbeat
        {
            get { return _isHeartbeat; }
            set { _isHeartbeat = value; }
        }
    }

    /// <summary>
    /// 客户端白名单
    /// </summary>
    public class ClientWhite
    {
        /// <summary>
        /// 客户端白名单 127.0.0.1,192.168.1.1
        /// </summary>
        public string ClientIp { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secretkey { get; set; }
        /// <summary>
        /// 加密方式
        /// </summary>
        public EncryptType encryptType { get; set; }
        /// <summary>
        /// 支持功能  ALL：全部   40000,12222,3334
        /// </summary>
        public string FuncList { get; set; }
    }
}
