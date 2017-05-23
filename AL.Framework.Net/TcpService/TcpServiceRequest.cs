using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zzll.Net.Framework.TcpService
{
    /// <summary>
    /// TcpService 请求对象
    /// </summary>
    public class TcpServiceRequest : Request
    {
        /// <summary>
        /// 会话上下文
        /// </summary>
        public Session client { get; set; }
    }
}
