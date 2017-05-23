using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zzll.Net.Framework.Timing
{
    /// <summary>
    /// 定时处理服务配置
    /// </summary>
    public class Config
    {
        public Config()
        {
            timing = new List<TimingExec>();
        }
        /// <summary>
        /// 定期执行时间
        /// </summary>
        public List<TimingExec> timing { get; set; }
    }

    /// <summary>
    /// 定时处理配置
    /// </summary>
    [Serializable]
    public class TimingExec
    {
        /// <summary>
        /// 功能码
        /// </summary>
        public string funCode { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime execTime { get; set; }
        /// <summary>
        /// 最后执行事件
        /// </summary>
        public DateTime lastExevTime { get; set; }
        /// <summary>
        /// 间隔时间(秒)
        /// </summary>
        public int Interval { get; set; }
        /// <summary>
        /// 是否真正运行
        /// </summary>
        [XmlIgnore]
        public bool IsRun = false;
        /// <summary>
        /// 定时类型
        /// </summary>
        public TimingExecType type { get; set; }
        /// <summary>
        /// 定时监控类型
        /// </summary>
        public enum TimingExecType
        {
            /// <summary>
            /// 间隔多少秒执行
            /// </summary>
            Interval,
            /// <summary>
            /// 周期 每天执行
            /// </summary>
            Day,
            /// <summary>
            /// 周期 每周执行
            /// </summary>
            week,
            /// <summary>
            /// 周期 每月执行
            /// </summary>
            month
        }
    }
}
