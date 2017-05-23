using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zzll.Net.Framework.Helper
{
    /// <summary>
    /// 工具类
    /// </summary>
   public class Utils
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(2000, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public static long GetReqId()
        {
            string stmap = GetTimeStamp();
            return Convert.ToInt64(stmap + DateTime.Now.ToString("fff"));
        }
    }
}
