#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：请求方式 (枚举)
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AL.Common.Helper.Http
{
    /// <summary>
    /// 请求方式
    /// </summary>
    public enum HttpMethod
    {
        GET,
        POST,
        HEAD,
        TRACE,
        PUT,
        DELETE,
        OPTIONS,
        CONNECT
    }
}
