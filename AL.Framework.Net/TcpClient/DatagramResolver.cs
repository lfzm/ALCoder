using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zzll.Net.Framework.TcpClient
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
        /// 接收报文片段存储
        /// </summary>
        public StringBuilder Datagram = new StringBuilder();

        /// <summary>
        /// 粘包处理
        /// </summary>
        /// <param name="Packet">报文</param>
        /// <returns>返回报文</returns>
        public virtual string[] Resolve(string Packet)
        {
            //加上上次通讯剩余的报文片断
            Packet = this.Datagram + Packet;
            if (string.IsNullOrEmpty(Suffix))
                return new string[1] { Packet };
            //分组取出报文
            string[] packets = Packet.Split(Suffix.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //判断最后一个报文是否有未结束符
            int LastIndex = Packet.LastIndexOf(Suffix);
            if (Packet.Length - Suffix.Length != LastIndex)
            {
                Datagram.Clear();
                Datagram.Append(packets[packets.Length - 1]);

                //移除不完全的报文
                List<string> list = packets.ToList();
                list.RemoveAt(packets.Length - 1);
                //转化为数组
                packets = list.ToArray();
            }
            else
                Datagram.Clear();

            return packets;
        }
    }
}
