using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zzll.Net.Framework.TcpService
{
    /// <summary>
    /// 报文解析器
    /// </summary>
    public class DatagramResolver
    {
        private string suffix = "";
        /// <summary>
        /// 报文后缀
        /// </summary>
        public string Suffix
        {
            get { return suffix; }
            set { suffix = value; }
        }

        /// <summary>
        /// 粘包处理
        /// </summary>
        /// <param name="s">会话上下文</param>
        /// <returns>返回报文</returns>
        public virtual string[] Resolve(Session s)
        {
            //加上上次通讯剩余的报文片断
            if (string.IsNullOrEmpty(Suffix))
                return new string[1] { s.Datagram.ToString() };
            //分组取出报文
            string[] packets = s.Datagram.ToString().Split(Suffix.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //判断最后一个报文是否有未结束符
            int LastIndex = s.Datagram.ToString().LastIndexOf(Suffix);
            if (s.Datagram.ToString().Length - Suffix.Length != LastIndex)
            {
                s.Datagram.Clear();
                s.Datagram.Append(packets[packets.Length - 1]);

                //移除不完全的报文
                List<string> list = packets.ToList();
                list.RemoveAt(packets.Length - 1);
                //转化为数组
                packets = list.ToArray();
            }
            else
                s.Datagram.Clear();

            return packets;
        }
    }
}
