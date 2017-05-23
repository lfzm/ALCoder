#region Copyright (C) 2017 AL系列开源项目


/***************************************************************************
*　　	文件功能描述：通用返回结果实体
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion

using System;
using AL.Common.Extention;

namespace AL.Common.Models
{
    /// <summary>
    /// 结果实体
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 空构造函数
        /// </summary>
        public Result() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">返回消息</param>
        public Result(string message):this(message,false,ResultTypes.ProcessingFailed)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ret"></param>
        public Result(string message, int ret) : this(message, ((ret == (int)ResultTypes.Success)) ? true : false, ret)
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ret"></param>
        public Result(string message,ResultTypes ret) : this(message, ((ret == ResultTypes.Success)) ? true : false, ret)
        {

        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">处理消息</param>
        /// <param name="success">是否成功</param>
        public Result(string message, bool success)
        {
            this.Message = message;
            this.Success = success;
            if (success)
                this.Status = (int)ResultTypes.Success;
            else
                this.Status = (int)ResultTypes.ProcessingFailed;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">处理消息</param>
        /// <param name="success">是否成功</param>
        /// <param name="ret">返回状态码</param>
        public Result(string message, bool success, ResultTypes ret)
        {
            this.Message = message;
            this.Success = success;
            this.Status = (int)ret;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">处理消息</param>
        /// <param name="success">是否成功</param>
        /// <param name="Status">返回状态码</param>
        protected Result(string message, bool success, int Status)
        {
            this.Message = message;
            this.Success = success;
            this.Status = Status;
        }

        /// <summary>
        /// 返回结果
        /// 一般情况下：
        ///  2xx   成功相关状态（如： 200）
        ///  3xx   参数相关错误 
        ///  4xx   用户授权相关错误
        ///  5xx   服务器内部相关错误信息
        ///  6xx   系统级定制错误信息，如升级维护等
        /// 也可依据第三方自行定义数值
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 错误或者状态
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
    }
  
    /// <summary>
    /// 自定义泛型的结果实体
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public class Result<TType> : Result
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Result()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data"></param>
        public Result(TType data)
            : base(null,ResultTypes.Success)
        {
            Data = data;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="message"></param>
        public Result(string message ,int ret)
            : base(message, ret)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="message"></param>
        public Result(string message,ResultTypes ret)
            : base(message,ret)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ret"></param>
        /// <param name="data">结果类型数据</param>
        public Result(string message, ResultTypes ret,TType data)
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
        public Result(string message, int ret, TType data)
            : base(message, ret)
        {
            Data = data;
        }

        /// <summary>
        ///  结果类型数据
        /// </summary>
        public TType Data { get; set; }
    }

}
