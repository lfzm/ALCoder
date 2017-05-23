#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：Http请求方法异常处理类
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
using AL.Common.Modules.LogModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AL.Common.Helper.Http
{
    /// <summary>
    /// HttpRequest请求错误异常
    /// </summary>
    public class HttpRequestException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Message">错误消息</param>
        public HttpRequestException(string Message) : this(Message, null) { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Message">错误消息</param>
        /// <param name="inner">错误对象</param>
        public HttpRequestException(string Message, Exception inner)
          : base(Message, inner)
        {
            LogHelper.Error(this.Message, "", inner);
        }
        /// 构造函数
        /// </summary>
        public HttpRequestException()
        {
            LogHelper.Error(this.Message, "", (Exception)this);
        }
    }
}
