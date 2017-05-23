#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：全局模块配置内
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion

using System;
using AL.Common.Modules.CacheModule;
using AL.Common.Modules.ConfigModule;
using AL.Common.Modules.LogModule;

namespace AL.Common.Modules
{
    /// <summary>
    /// 基础配置模块
    /// </summary>
    public static class ModuleConfig
    {
        #region  Module初始化模块
        /// <summary>
        /// 日志模块提供者
        /// </summary>
        public static Func<string, ILogWriter> LogWriterProvider { get; set; }

        /// <summary>
        /// 缓存模块提供者
        /// </summary>
        public static Func<string, ICache> CacheProvider { get; set; }
        
        /// <summary>
        ///  配置信息模块提供者
        /// </summary>
        public static Func<string, IConfig> DirConfigProvider { get; set; }
        #endregion


    }
}
