#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：配置处理缓存辅助类
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
using System;
using System.Collections.Concurrent;
using AL.Common.Models;

namespace AL.Common.Modules.ConfigModule
{
    /// <summary>
    /// 字典配置通用存储获取信息
    /// </summary>
    public static class ConfigHelper
    {
        private static readonly ConcurrentDictionary<string, IConfig> _dirConfigDirs = new ConcurrentDictionary<string, IConfig>();

        /// <summary>
        /// 通过模块名称获取
        /// </summary>
        /// <param name="dirConfigModule"></param>
        /// <returns></returns>
        private static IConfig GetConfig(string dirConfigModule)
        {
            if (string.IsNullOrEmpty(dirConfigModule))
                dirConfigModule = ModuleNames.Default;

            if (_dirConfigDirs.ContainsKey(dirConfigModule))
                return _dirConfigDirs[dirConfigModule];

            IConfig dirConfig;
            if (ModuleConfig.DirConfigProvider != null)
                dirConfig = ModuleConfig.DirConfigProvider.Invoke(dirConfigModule) ?? new Config();
            else
                dirConfig = new Config();
            _dirConfigDirs.TryAdd(dirConfigModule, dirConfig);
            return dirConfig;
        }


        /// <summary>
        /// 设置字典配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dirConfig"></param>
        /// <param name="moduleName">模块名称</param>
        /// <typeparam name="TConfig"></typeparam>
        /// <returns></returns>
        public static Result SetConfig<TConfig>(string key, TConfig dirConfig,
            string moduleName = null) where TConfig : class, new()
        {
            return GetConfig(moduleName).SetConfig(key, dirConfig);
        }

        /// <summary>
        ///   获取字典配置
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="key"></param>
        /// <param name="moduleName">模块名称</param>
        /// <returns></returns>
        public static TConfig GetConfig<TConfig>(string key, string moduleName = null) where TConfig : class, new()
        {
            return GetConfig(moduleName).GetConfig<TConfig>(key);
        }

        /// <summary>
        ///  移除配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="moduleName">模块名称</param>
        /// <returns></returns>
        public static Result RemoveConfig(string key, string moduleName = null)
        {
            return GetConfig(moduleName).RemoveConfig(key);
        }
    }
}