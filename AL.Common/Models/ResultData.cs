using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AL.Common.Models
{
    /// <summary>
    /// 结果带String数据
    /// </summary>
    public class ResultData : Result
    {
        /// </summary>
        public ResultData()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data"></param>
        public ResultData(string data)
            : base(null, ResultTypes.Success)
        {
            Data = data;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="message"></param>
        public ResultData(string message, int ret)
            : base(message, ret)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="message"></param>
        public ResultData(string message, ResultTypes ret)
            : base(message, ret)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ret"></param>
        /// <param name="data">结果类型数据</param>
        public ResultData(string message, ResultTypes ret, string data)
            : base(message, ret)
        {
            Data = data;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ret"></param>
        /// <param name="data">结果类型数据</param>
        public ResultData(string message, int ret, string data)
            : base(message, ret)
        {
            Data = data;
        }

        /// <summary>
        ///  结果类型数据
        /// </summary>
        public string Data { get; set; }
    }
}
