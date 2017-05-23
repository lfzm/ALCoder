using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zzll.Net.Framework.RedisMQ
{
    /// <summary>
    /// RedisMQ消息队列配置
    /// </summary>
    public class Config
    {
        public Config()
        {
            redis = new List<Redis>();
        }
        /// <summary>
        /// Redis配置
        /// </summary>
        public List<Redis> redis { get; set; }
    }

    public class Redis
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectString { get; set; }
        /// <summary>
        /// 功能吗
        /// </summary>
        public string func { get; set; }
        /// <summary>
        /// 监听RedisKey
        /// </summary>
        public string redisKey { get; set; }
        /// <summary>
        /// 库号
        /// </summary>
        public int dbNum { get; set; }
    }

}
