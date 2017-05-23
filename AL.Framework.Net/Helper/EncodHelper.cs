using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zzll.Net.Framework.Helper
{
    /// <summary>
    /// 编码模式
    /// </summary>
    public class EncodHelper
    {
        /// <summary>
        /// 编码方式
        /// </summary>
        private EncodingMothord _encodingMothord;

        #region 构造方法
        public EncodHelper() { }
        /// <summary>
        /// 编码方式
        /// </summary>
        /// <param name="encodingMothord"></param>
        public EncodHelper(EncodingMothord encodingMothord)
        {
            _encodingMothord = encodingMothord;
        }
        #endregion
        
        /// <summary>
        /// 通讯数据解码
        /// </summary>
        /// <param name="dataBytes">需要解码的数据</param>
        /// <param name="size">需要解密的数据大小</param>
        /// <returns>编码后的数据</returns>
        public string ToString(byte[] dataBytes, int size)
        {
            switch (_encodingMothord)
            {
                case EncodingMothord.Default:
                    {
                        return Encoding.Default.GetString(dataBytes, 0, size);
                    }
                case EncodingMothord.Unicode:
                    {
                        return Encoding.Unicode.GetString(dataBytes, 0, size);
                    }
                case EncodingMothord.UTF8:
                    {
                        return Encoding.UTF8.GetString(dataBytes, 0, size);
                    }
                case EncodingMothord.ASCII:
                    {
                        return Encoding.ASCII.GetString(dataBytes, 0, size);
                    }
                case EncodingMothord.GBK:
                    {
                        return Encoding.GetEncoding("GBK").GetString(dataBytes, 0, size);
                    }
                default:
                    {
                        return Encoding.Default.GetString(dataBytes, 0, size);
                    }
            }
        }

        /// <summary>
        /// 通讯数据编码
        /// </summary>
        /// <param name="datagram">需要编码的报文</param>
        /// <returns>编码后的数据</returns>
        public byte[] ToString(string datagram)
        {
            switch (_encodingMothord)
            {
                case EncodingMothord.Default:
                    {
                        return Encoding.Default.GetBytes(datagram);
                    }
                case EncodingMothord.Unicode:
                    {
                        return Encoding.Unicode.GetBytes(datagram);
                    }
                case EncodingMothord.UTF8:
                    {
                        return Encoding.UTF8.GetBytes(datagram);
                    }
                case EncodingMothord.ASCII:
                    {
                        return Encoding.ASCII.GetBytes(datagram);
                    }
                case EncodingMothord.GBK:
                    {
                        return Encoding.GetEncoding("GBK").GetBytes(datagram);
                    }
                default:
                    {
                        return Encoding.Default.GetBytes(datagram);
                    }
            }
        }

        /// <summary>
        /// 获取对应的编码
        /// </summary>
        /// <returns></returns>
        public Encoding ToEncoding()
        {
            switch (_encodingMothord)
            {
                case EncodingMothord.Default:
                    {
                        return Encoding.Default;
                    }
                case EncodingMothord.Unicode:
                    {
                        return Encoding.Unicode;
                    }
                case EncodingMothord.UTF8:
                    {
                        return Encoding.UTF8;
                    }
                case EncodingMothord.ASCII:
                    {
                        return Encoding.ASCII;
                    }
                case EncodingMothord.GBK:
                    {
                        return Encoding.GetEncoding("GBK");
                    }
                default:
                    {
                        return Encoding.Default;
                    }
            }
        }

    }

    /// <summary>
    /// 编码枚举
    /// </summary>
    public enum EncodingMothord
    {
        /// <summary>
        /// 系统默认
        /// </summary>
        Default = 0,
        /// <summary>
        /// Unicode编码
        /// </summary>
        Unicode = 1,
        /// <summary>
        /// UTF8编码
        /// </summary>
        UTF8 = 2,
        /// <summary>
        /// ASCII编码
        /// </summary>
        ASCII = 3,
        /// <summary>
        /// GBK编码
        /// </summary>
        GBK = 4,
    }

}
