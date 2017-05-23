#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：请求内容类型 (枚举)
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
    /// 请求内容类型
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// application/json
        /// </summary>
        json,
        /// <summary>
        /// application/x-www-form-urlencoded
        /// </summary>
        x_www_form_urlencoded
    }
}
