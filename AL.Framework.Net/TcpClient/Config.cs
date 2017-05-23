using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zzll.Net.Framework.Helper;

namespace Zzll.Net.Framework.TcpClient
{
    /// <summary>
    /// TcpClient 配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort Port { get; set; }
        /// <summary>
        /// 是否开启长连接 
        /// </summary>
        public bool IsLongConnection { get; set; }
        /// <summary>
        /// 编码格式
        /// </summary>
        public EncodingMothord Encoding { get; set; }
    }
}
